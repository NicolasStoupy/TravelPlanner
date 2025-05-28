using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Extensions;
using BussinessLogic.Interfaces;
using BussinessLogic.Services.ServicesStatus;
using Commons;
using Commons.ErrorsHandlings;
using Commons.Models;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace BussinessLogic.Services
{
    /// <summary>
    /// Provides business logic for managing travel data, including creation, update, deletion,
    /// note management, and media handling. This service abstracts access to the underlying database
    /// and file system using Entity Framework Core and AutoMapper.
    /// </summary>
    /// <remarks>
    /// This service uses <see cref="IDbContextFactory{TContext}"/> to manage database contexts in a scoped and thread-safe way,
    /// and maps between infrastructure entities and domain models using <see cref="IMapper"/>.
    /// Media operations such as file storage and replacement are handled via the <see cref="DocumentProvider"/>.
    /// </remarks>
    public class TravelService(
        IDbContextFactory<TravelPlannerContext> context,
        IMapper mapper,
        DocumentProvider document,
        IMediaService mediaService, ILogger<TravelService> logger) : ITravelService
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context = context;
        private readonly DocumentProvider _document = document;
        private readonly IMapper _mapper = mapper;
        private readonly IMediaService _mediaService = mediaService;
        private readonly ILogger<TravelService> _logger = logger;

        public ServiceResult<Travel, TravelServiceStatus> GetTravel(int travelID)
        {
            try
            {
                if (travelID <= 0)
                    return new ErrorResult<Travel, TravelServiceStatus>
                        (TravelServiceStatus.InvalidTravelId);

                using var ctx = _context.CreateDbContext();
                var entity = ctx.Trips.FirstOrDefault(t => t.TripId == travelID);

                if (entity == null)
                    return new ErrorResult<Travel, TravelServiceStatus>
                        (TravelServiceStatus.TravelNotFound);

                var travel = _mapper.Map<Travel>(entity);
                var mes = new SuccessResult<Travel, TravelServiceStatus>(travel);
                return mes;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving travel with ID {TravelID}", travelID);
                return new ErrorResult<Travel, TravelServiceStatus>(TravelServiceStatus.UnknownError);
            }
        }

        public async Task<ServiceResult<List<Travel>, TravelServiceStatus>> GetTravels(bool includeActivity = false, bool includeNotes = false, bool includeFollowers = false)
        {
            using var context = _context.CreateDbContext();
            var trips = await context.Trips.OrderBy(t => t.CreatedAt).ToListAsync();

            if (!_mapper.TryMap(trips, out List<Travel> travelItems, _logger))
            {
                return new ErrorResult<List<Travel>, TravelServiceStatus>(TravelServiceStatus.UnknownError);
            }

            return new SuccessResult<List<Travel>, TravelServiceStatus>(travelItems);
        }

        public async Task<ServiceResult<bool, TravelServiceStatus>> AddMediaToTravel(
            List<byte[]> medias, int travelID, Commons.TypeMedia mediaType)
        {
            try
            {
                if (travelID <= 0)
                    return new ErrorResult<bool, TravelServiceStatus>
                        (TravelServiceStatus.InvalidTravelId);
                if (medias.Count == 0)
                    return new ErrorResult<bool, TravelServiceStatus>
                        (TravelServiceStatus.NoMedia);

                using var context = _context.CreateDbContext();

                var trip = context.Trips.FirstOrDefault(t => t.TripId == travelID);

                if ((trip == null))
                    return new ErrorResult<bool, TravelServiceStatus>
                        (TravelServiceStatus.TravelNotFound);

                var savedFilesGuid = _mediaService.SaveMedias(medias, mediaType);

                //if some files failed to save (already saved files will be rolled back)
                if (savedFilesGuid != null && savedFilesGuid.Count() != medias.Count)
                {
                    foreach (var item in savedFilesGuid)
                    {
                        _document.RemoveFile(item, mediaType);
                    }
                    return new ErrorResult<bool, TravelServiceStatus>
                                            (TravelServiceStatus.ErrorWhenAddingFile);
                }
                if (savedFilesGuid != null)
                {
                    foreach (var fileGuid in savedFilesGuid)
                    {
                        trip.Media.Add(new Medium
                        {
                            FileGuid = fileGuid,
                            Description = string.Empty,
                            MediaType = 1
                        });
                    }
                }

                await context.SaveChangesAsync();

                return new SuccessResult<bool, TravelServiceStatus>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout de medias au voyage {travelID}", travelID);
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.UnknownError);
            }
        }

        public async Task<ServiceResult<bool, TravelServiceStatus>> SaveTravel(Travel travel)
        {
            if (travel == null)
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.InvalidTravelId);

            using var context = _context.CreateDbContext();

            if (!_mapper.TryMap(travel, out Trip trip, _logger))
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.UnknownError);

            //trip.CurrencyCode = travel.currencie;

            Guid? savedFileGuid = null;
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                // 1. Save The trip without image
                context.Trips.Add(trip);
                await context.SaveChangesAsync();

                // 2. Save Image
                if (travel.image != null)
                {
                    _document.SetMediaType(Commons.TypeMedia.Images);
                    var savedImageGuid = _document.SaveFile(travel.image);
                    if (savedImageGuid == null)
                    {
                        transaction.Rollback();
                        return new ErrorResult<bool, TravelServiceStatus>(
                            TravelServiceStatus.ErrorWhenAddingFile);
                    }

                    // 3. Update the trip information
                    trip.TripBackgroundGuid = savedImageGuid;
                    context.Trips.Update(trip);
                    await context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return new SuccessResult<bool, TravelServiceStatus>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout d'un voyage {travelID}", travel.Id);
                await transaction.RollbackAsync();
                //  remove orphan file
                if (savedFileGuid.HasValue)
                {
                    _document.RemoveFile(savedFileGuid.Value, Commons.TypeMedia.Images);
                }
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.UnknownError);
            }
        }

        public async Task<ServiceResult<bool, TravelServiceStatus>> DeleteTravel(int travelID)
        {
            if (travelID < 0)
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.InvalidTravel);

            using var context = _context.CreateDbContext();
            try
            {
                var trip = await context.Trips
                    .Include(t => t.LogBooks)
                    .Include(t => t.Media)
                    .Include(t => t.Activities).ThenInclude(a => a.LogBooks)
                    .Include(t => t.Activities).ThenInclude(a => a.ActivityCosts)
                    .Include(t => t.Activities).ThenInclude(a => a.Attendees)
                    .FirstOrDefaultAsync(t => t.TripId == travelID);

                if (trip == null)
                    return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.TravelNotFound);

                var medias = trip.Media;
                var mediaCount = medias.Count();
                Dictionary<Guid, byte[]> memoryFiles = new Dictionary<Guid, byte[]>();
                //foreach (var media in medias)
                //{
                //    var loadedFile = _document.GetFile(media.FileGuid, TypeMedia.Images);
                //    if (loadedFile != null)
                //    {
                //        memoryFiles.Add(media.FileGuid, loadedFile);
                //    }
                //    if (_document.RemoveFile(media.FileGuid, TypeMedia.Images))
                //    {
                //        mediaCount -= 1;
                //    }
                //}
                //if (mediaCount != 0)
                //{
                //    foreach (var memoryFile in memoryFiles)
                //    {
                //        _document.ReplaceFile(memoryFile.Key, memoryFile.Value);
                //    }
                //    return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.ErrorWhenRemovingFile);
                //}
                context.RemoveRange(trip.Media);

                foreach (var activity in trip.Activities)
                {
                    // Supprimer les LogBooks des Activity
                    context.LogBooks.RemoveRange(activity.LogBooks);

                    foreach (var activityCost in activity.ActivityCosts)
                    {
                        // Supprimer les Media des ActivityCost
                        context.Media.RemoveRange(activityCost.Media);
                    }

                    // Supprimer les Attendees
                    context.Attendees.RemoveRange(activity.Attendees);

                    // Supprimer les ActivityCosts
                    context.ActivityCosts.RemoveRange(activity.ActivityCosts);
                    context.Remove(activity);
                }

                context.Remove(trip);
                await context.SaveChangesAsync();
                return new SuccessResult<bool, TravelServiceStatus>(true);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred when the system trying to delete a travel");
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.UnknownError);
            }
        }

        public async Task<ServiceResult<bool, TravelServiceStatus>> UpdateTravel(Travel travel)
        {
            if (travel == null)
                return new ErrorResult<bool, TravelServiceStatus>
                    (TravelServiceStatus.InvalidTravel);

            using var context = _context.CreateDbContext();

            if (!_mapper.TryMap(travel, out Trip trip, _logger))
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.UnknownError);

            Guid? savedFileGuid = null;

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var existingTrip = await context.Trips.FindAsync(trip.TripId);
                if (existingTrip == null)
                {
                    return new ErrorResult<bool, TravelServiceStatus>
                                      (TravelServiceStatus.TravelNotFound);
                }

                // 1. Update les propriétés (hors image pour le moment)
                context.Entry(existingTrip).CurrentValues.SetValues(trip);

                await context.SaveChangesAsync();

                // 2. Save nouvelle image si nécessaire
                if (travel?.image != null)
                {
                    _document.SetMediaType(Commons.TypeMedia.Images);
                    savedFileGuid = _document.ReplaceFile(travel.imageID, travel.image);

                    // 3. Mise à jour du GUID image
                    existingTrip.TripBackgroundGuid = savedFileGuid;
                    context.Trips.Update(existingTrip);
                    await context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return new SuccessResult<bool, TravelServiceStatus>(true);
            }
            catch (Exception ex)
            {
                await transaction.RollbackAsync();

                // Suppression de l'image si elle avait été enregistrée
                if (savedFileGuid.HasValue)
                {
                    _document.RemoveFile(savedFileGuid.Value, Commons.TypeMedia.Images);
                }
                _logger.LogError(ex, "Erreur lors de l'édition d'un voyage {travelID}", travel?.Id);
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.UnknownError);
            }
        }

        public async Task<ServiceResult<bool, TravelServiceStatus>> CloneTravel(Travel travel)
        {
            // Validation de l'entrée
            if (travel == null)
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.InvalidTravelId);
            try
            {
                await using var ctx = _context.CreateDbContext();
                //Création du clone
                if (!_mapper.TryMap(travel, out Trip tripCloned, _logger))
                    return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.UnknownError);

                tripCloned.TripId = 0;                     // Reset de la PK
                ctx.Trips.Add(tripCloned);
                //Persistance
                await ctx.SaveChangesAsync();
                //Succès
                return new SuccessResult<bool, TravelServiceStatus>(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Erreur lors de la sauvegarde");
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.DatabaseError);
            }
            catch (Exception ex)
            {
                //Log et erreur
                _logger.LogError(ex, "Erreur lors du clonage du voyage (ID original: {TravelId})", travel.Id);
                return new ErrorResult<bool, TravelServiceStatus>(TravelServiceStatus.UnknownError);
            }
        }

        public List<MemoryFile> GetMemories(int id, TypeMedia mediaType)
        {
            var result = new List<MemoryFile>();
            using var context = _context.CreateDbContext();
            var medias = context.Media.Where(m => m.TripId == id && m.ActivityCostId == null);
            foreach (var media in medias)
            {
                result.Add(new MemoryFile()
                {
                    Files = _document.GetFile(media.FileGuid, mediaType),
                    Description = media.Description,
                    FileID = media.MediaId,
                    FileGuid = media.FileGuid
                });
            }

            return result;
        }

        public async Task<Result> RemoveMemories(IEnumerable<MemoryFile>? selectedMemories, int travelID)
        {
            using var context = _context.CreateDbContext();
            var trip = context.Trips.FirstOrDefault(t => t.TripId == travelID);
            if (trip != null && selectedMemories != null)
            {
                foreach (var memory in selectedMemories)
                {
                    var media = context.Media.FirstOrDefault(m => m.MediaId == memory.FileID);
                    if (media != null)
                    {
                        if (_document.RemoveFile(media.FileGuid, TypeMedia.Images))
                            context.Media.Remove(media);
                    }
                }
            }
            await context.SaveChangesAsync();

            return Result.Success("Success");
        }

        public Result UpdateMemory(MemoryFile memory)
        {
            using var context = _context.CreateDbContext();

            var media = context.Media.FirstOrDefault(m => m.MediaId == memory.FileID);
            if (media != null)
            {
                media.Description = memory.Description ?? string.Empty;
                context.SaveChanges();
            }
            return Result.Success();
        }
    }
}
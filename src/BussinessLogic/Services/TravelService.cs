using AutoMapper;
using BussinessLogic.Entities;
using BussinessLogic.Extensions;
using BussinessLogic.Interfaces;
using BussinessLogic.Models;
using Commons;
using Commons.ErrorsHandlings;
using Commons.Resources;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using QuestPDF.Fluent;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

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
        private readonly ILogger<TravelService> _logger = logger;
        private readonly IMapper _mapper = mapper;
        private readonly IMediaService _mediaService = mediaService;

        /// <summary>
        /// Asynchronously adds one or more media files to the specified travel record.
        /// </summary>
        /// <param name="medias">
        /// A list of byte arrays, each representing a media file to add. Must contain at least one element.
        /// </param>
        /// <param name="travelID">
        /// The unique identifier of the travel record to which media will be attached. Must be greater than zero.
        /// </param>
        /// <param name="mediaType">
        /// The type of the media being added (e.g., image, video, document).
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> if all media files were saved and associated successfully.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.INVALID_TRAVEL_ID"/> if <paramref name="travelID"/> is less than or equal to zero.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.NO_MEDIA"/> if <paramref name="medias"/> is empty.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.TRAVEL_NOT_FOUND"/> if no travel record exists for the given <paramref name="travelID"/>.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.ERROR_WHEN_ADDING_FILE"/> if one or more files failed to save (any partial saves will be rolled back).
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        public async Task<ServiceResult<bool>> AddMediaToTravel(
            List<byte[]> medias, int travelID, Commons.TypeMedia mediaType)
        {
            if (travelID <= 0)
                return ServiceResult<bool>.Failure(TravelServiceMessage.INVALID_TRAVEL_ID);
            if (medias.Count == 0)
                return ServiceResult<bool>.Failure(TravelServiceMessage.NO_MEDIA);

            using var context = _context.CreateDbContext();

            var trip = context.Trips.FirstOrDefault(t => t.TripId == travelID);

            if ((trip == null))
                return ServiceResult<bool>.Failure(TravelServiceMessage.TRAVEL_NOT_FOUND);

            var savedFilesGuid = _mediaService.SaveMedias(medias, mediaType);

            if (!savedFilesGuid.IsSuccess)
            {
                return ServiceResult<bool>.Failure(TravelServiceMessage.ERROR_WHEN_ADDING_FILE);
            }

            foreach (var fileGuid in savedFilesGuid.Value)
            {
                trip.Media.Add(new Medium
                {
                    FileGuid = fileGuid,
                    Description = string.Empty,
                    MediaType = 1
                });
            }

            await context.SaveChangesAsync();

            return ServiceResult<bool>.Success(true);
        }

        /// <summary>
        /// Asynchronously creates a duplicate of the specified <see cref="Travel"/> record.
        /// The clone will have a new primary key and identical property values (excluding the ID).
        /// </summary>
        /// <param name="travel">
        /// The <see cref="Travel"/> object to clone. Must not be null.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> if the travel record is cloned successfully.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.INVALID_TRAVEL"/> if <paramref name="travel"/> is null.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.UNKNOWN_ERROR"/> if mapping the <see cref="Travel"/> to a <see cref="Trip"/> entity fails.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.DATABASE_ERROR"/> if a database update exception occurs during save.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        /// <exception cref="DbUpdateException">
        /// Thrown if an error occurs while saving the cloned entity to the database;
        /// this exception is caught and translated into a failure <see cref="ServiceResult{Boolean}"/>.
        /// </exception>
        public async Task<ServiceResult<bool>> CloneTravel(Travel travel)
        {
            // Validation de l'entrée
            if (travel == null)
                return ServiceResult<bool>.Failure(TravelServiceMessage.INVALID_TRAVEL);
            try
            {
                await using var ctx = _context.CreateDbContext();
                //Création du clone
                if (!_mapper.TryMap(travel, out Trip tripCloned, _logger))
                    return ServiceResult<bool>.Failure(GlobalServiceMessage.UNKNOWN_ERROR);

                tripCloned.TripId = 0;                     // Reset de la PK
                var image = tripCloned.TripBackgroundGuid;

                if (image.HasValue)
                {
                    var img = _document.GetFile(image, typeMedia: TypeMedia.Images);
                    if (img == null)
                        return ServiceResult<bool>.Failure(TravelServiceMessage.ERROR_WHEN_ADDING_FILE);

                    var newFileGuid = _document.SaveFile(img);
                    if (newFileGuid.HasValue)
                    {
                        tripCloned.TripBackgroundGuid = newFileGuid;
                    }
                }
                ctx.Trips.Add(tripCloned);
                //Persistance
                await ctx.SaveChangesAsync();
                //Succès
                return ServiceResult<bool>.Success(true);
            }
            catch (DbUpdateException dbEx)
            {
                _logger.LogError(dbEx, "Erreur lors de la sauvegarde");
                return ServiceResult<bool>.Failure(GlobalServiceMessage.DATABASE_ERROR);
            }
        }

        /// <summary>
        /// Asynchronously deletes the specified travel record and all its related data,
        /// including media files, logbooks, activities, activity costs, and attendees.
        /// Ensures that media files are removed from storage, and rolls back removals if any file deletion fails.
        /// </summary>
        /// <param name="travelID">
        /// The unique identifier of the travel record to delete. Must be zero or greater.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> and message <see cref="TravelServiceMessage.TRAVEL_REMOVED"/>
        ///       if the travel and all related entities/files are deleted successfully.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.INVALID_TRAVEL_ID"/> if <paramref name="travelID"/> is less than zero.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.TRAVEL_NOT_FOUND"/> if no travel record exists with the given <paramref name="travelID"/>.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.ERROR_WHEN_REMOVING_FILE"/> if one or more media files could not be removed
        ///       (any successfully deleted files will be restored).
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        public async Task<ServiceResult<bool>> DeleteTravel(int travelID)
        {
            if (travelID < 0)
                return ServiceResult<bool>.Failure(TravelServiceMessage.INVALID_TRAVEL_ID);

            using var context = _context.CreateDbContext();

            var trip = await context.Trips
                .Include(t => t.LogBooks)
                .Include(t => t.Media)
                .Include(t => t.Activities).ThenInclude(a => a.LogBooks)
                .Include(t => t.Activities).ThenInclude(a => a.ActivityCosts)
                .Include(t => t.Activities).ThenInclude(a => a.Attendees)
                .FirstOrDefaultAsync(t => t.TripId == travelID);

            if (trip == null)
                return ServiceResult<bool>.Failure(TravelServiceMessage.TRAVEL_NOT_FOUND);

            var medias = trip.Media;
            var mediaCount = medias.Count();
            Dictionary<Guid, byte[]> memoryFiles = new Dictionary<Guid, byte[]>();

            var backgroundGuid = trip.TripBackgroundGuid;
            if (backgroundGuid.HasValue)
            {
                var backImage = _document.GetFile(backgroundGuid, TypeMedia.Images);
                if (backImage != null)
                {
                    if (!_document.RemoveFile(backgroundGuid, typeMedia: TypeMedia.Images))
                    {
                        _document.SaveFile(backImage);
                        return ServiceResult<bool>.Failure(TravelServiceMessage.ERROR_WHEN_REMOVING_FILE);
                    }
                }
            }
            foreach (var media in medias)
            {
                var loadedFile = _document.GetFile(media.FileGuid, TypeMedia.Images);
                if (loadedFile != null)
                {
                    memoryFiles.Add(media.FileGuid, loadedFile);
                }
                if (_document.RemoveFile(media.FileGuid, TypeMedia.Images))
                {
                    mediaCount -= 1;
                }
            }
            if (mediaCount != 0)
            {
                foreach (var memoryFile in memoryFiles)
                {
                    _document.ReplaceFile(memoryFile.Key, memoryFile.Value);
                }
                return ServiceResult<bool>.Failure(TravelServiceMessage.ERROR_WHEN_REMOVING_FILE);
            }
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
            return ServiceResult<bool>.Success(true, TravelServiceMessage.TRAVEL_REMOVED);
        }

        public ServiceResult<byte[]> ExportTravel(int travelID)
        {
            if (travelID <= 0)
                return ServiceResult<byte[]>.Failure(TravelServiceMessage.INVALID_TRAVEL_ID);

            using var context = _context.CreateDbContext();
            var travel = context.Trips
                .Include(a => a.Activities)
                .ThenInclude(t => t.ActivityCosts)
                .Include(l => l.LogBooks)
                .Include(c => c.Media)
                .FirstOrDefault(t => t.TripId == travelID);
            if (travel == null)
                return ServiceResult<byte[]>.Failure(TravelServiceMessage.TRAVEL_NOT_FOUND);
            // Load the medias from disk
            var mediaList = new Dictionary<Guid, byte[]>();
            foreach (var medium in travel.Media)
            {
                var mediaFile = _document.GetFile(medium.FileGuid, TypeMedia.Images);
                if (mediaFile != null)
                    mediaList.Add(medium.FileGuid, mediaFile);
            }
            // Load the default media for a trip
            if (travel.TripBackgroundGuid.HasValue)
            {
                var tripMediaFile = _document.GetFile(travel.TripBackgroundGuid, TypeMedia.Images);
                if (tripMediaFile != null)
                    mediaList.Add(travel.TripBackgroundGuid.Value, tripMediaFile);
            }

            var tBinModel = new TBinModel(); // creation of the model required for Export

            tBinModel.trip = travel;
            tBinModel.medias = mediaList;

            //Options for cyclic object understanding
            var options = new JsonSerializerOptions
            {
                ReferenceHandler = ReferenceHandler.Preserve,
                WriteIndented = true
            };

            //Serialize with options
            string jsonString = JsonSerializer.Serialize(tBinModel, options);

            //converting to URF8 encoding
            byte[] payload = Encoding.UTF8.GetBytes(jsonString);

            return ServiceResult<byte[]>.Success(payload);
        }

        public ServiceResult<byte[]> GeneratePdfSummary(Travel travel)
        {

            if (travel == null)
                return ServiceResult<byte[]>.Failure(MediaServiceMessages.INVALID_INPUT);



            var fullTravel = GetTravel(travel.Id);
            if (fullTravel.IsSuccess)
            {
                var document = new TravelDocumentPDF(fullTravel.Value);
                var pdf = document.GeneratePdf();
                return ServiceResult<byte[]>.Success(pdf);
            }
            return ServiceResult<byte[]>.Failure(GlobalServiceMessage.UNKNOWN_ERROR);
        }

        /// <summary>
        /// Retrieves all memory files for the specified travel record.
        /// </summary>
        /// <param name="travelID">
        /// The unique identifier of the travel record. Must be greater than zero.
        /// </param>
        /// <param name="mediaType">
        /// The type of media to retrieve (e.g., images, videos).
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Success"/> with a <see cref="List{MemoryFile}"/> of files (possibly empty) if retrieval succeeds.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Failure"/> with
        ///       <see cref="TravelServiceMessage.INVALID_TRAVEL_ID"/> if <paramref name="travelID"/> is less than or equal to zero.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Failure"/> with
        ///       <see cref="TravelServiceMessage.TRAVEL_NOT_FOUND"/> if no travel record exists for the given <paramref name="travelID"/>.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Failure"/> with
        ///       <see cref="TravelServiceMessage.UNKNOWN_ERROR"/> if an unexpected error occurs during retrieval.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        public ServiceResult<List<MemoryFile>> GetMemories(int travelID, TypeMedia mediaType)
        {
            if (travelID <= 0)
                return ServiceResult<List<MemoryFile>>
                    .Failure(TravelServiceMessage.INVALID_TRAVEL_ID);

            using var context = _context.CreateDbContext();

            var trip = context.Trips.Find(travelID);
            if (trip == null)
                return ServiceResult<List<MemoryFile>>
                    .Failure(TravelServiceMessage.TRAVEL_NOT_FOUND);

            var medias = context.Media
                .Where(m => m.TripId == travelID && m.ActivityCostId == null)
                .ToList();

            var result = new List<MemoryFile>();
            foreach (var media in medias)
            {
                var fileBytes = _document.GetFile(media.FileGuid, mediaType);
                result.Add(new MemoryFile
                {
                    Files = fileBytes,
                    Description = media.Description,
                    FileID = media.MediaId,
                    FileGuid = media.FileGuid
                });
            }

            return ServiceResult<List<MemoryFile>>.Success(result);
        }

        /// <summary>
        /// Retrieves a <see cref="Travel"/> instance by its identifier.
        /// </summary>
        /// <param name="travelID">
        /// The unique identifier of the travel record to retrieve. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Success"/> with the <see cref="Travel"/> instance if found and mapped successfully.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Warning"/> with
        ///       <see cref="TravelServiceMessage.INVALID_TRAVEL_ID"/> if <paramref name="travelID"/> is less than or equal to zero.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Warning"/> with
        ///       <see cref="TravelServiceMessage.TRAVEL_NOT_FOUND"/> if no matching record exists.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Warning"/> with
        ///       <see cref="TravelServiceMessage.UNKNOWN_ERROR"/> if an error occurs during mapping.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        public ServiceResult<Travel> GetTravel(int travelID)
        {
            if (travelID <= 0)
                return ServiceResult<Travel>
                    .Warning(TravelServiceMessage.INVALID_TRAVEL_ID);

            using var ctx = _context.CreateDbContext();
            var entity = ctx.Trips.FirstOrDefault(t => t.TripId == travelID);

            if (entity == null)
                return ServiceResult<Travel>
                    .Warning(TravelServiceMessage.TRAVEL_NOT_FOUND);

            Action<IMappingOperationOptions> mappingOptions = opts =>
            {
                opts.Items[MappingContextExclusion.Activities.ToString()] = true;
                opts.Items[MappingContextExclusion.Notes.ToString()] = true;
                opts.Items[MappingContextExclusion.Followers.ToString()] = true;
                opts.Items[MappingContextExclusion.Memories.ToString()] = true;
            };

            if (!_mapper.TryMap(entity, out Travel travel, _logger,mappingOptions))
                return ServiceResult<Travel>
                    .Warning(GlobalServiceMessage.UNKNOWN_ERROR);

            return ServiceResult<Travel>.Success(travel);
        }

        /// <summary>
        /// Asynchronously retrieves all <see cref="Travel"/> records, ordered by creation date.
        /// </summary>
        /// <param name="includeActivity">
        /// If <c>true</c>, include related activity data. Currently reserved for future use.
        /// </param>
        /// <param name="includeNotes">
        /// If <c>true</c>, include related notes data. Currently reserved for future use.
        /// </param>
        /// <param name="includeFollowers">
        /// If <c>true</c>, include related follower data. Currently reserved for future use.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{T}"/> containing:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Success"/> with a <see cref="List{Travel}"/> of all travels if mapping succeeds.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{T}.Warning"/> with
        ///       <see cref="TravelServiceMessage.UNKNOWN_ERROR"/> if an error occurs during mapping.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        public async Task<ServiceResult<List<Travel>>> GetTravels(
                bool includeActivity = false,
                bool includeNotes = false,
                bool includeFollowers = false,
                bool includeMemories =false)
        {
            using var context = _context.CreateDbContext();
            var trips = await context.Trips.OrderBy(t => t.CreatedAt).ToListAsync();

            Action<IMappingOperationOptions> mappingOptions = opts =>
            {
                opts.Items[MappingContextExclusion.Activities.ToString()] = includeActivity;
                opts.Items[MappingContextExclusion.Notes.ToString()] = includeNotes;
                opts.Items[MappingContextExclusion.Followers.ToString()] = includeFollowers;
                opts.Items[MappingContextExclusion.Memories.ToString()] = includeMemories;
            };

            if (!_mapper.TryMap(trips, out List<Travel> travelItems, _logger,mappingOptions))
            {
                return ServiceResult<List<Travel>>
                   .Warning(GlobalServiceMessage.UNKNOWN_ERROR);
            }

            return ServiceResult<List<Travel>>.Success(travelItems);
        }

        /// <summary>
        /// Imports a travel package from a serialized binary file and persists it to the database.
        /// </summary>
        /// <param name="travelFile">
        /// A byte array containing the UTF-8 encoded representation of a <see cref="TBinModel"/>,
        /// which includes the trip data and associated media blobs.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating whether the import succeeded.
        /// On success, the <c>Data</c> property will be <c>true</c>. On failure, an appropriate
        /// error message will be returned (e.g., invalid file format or database error).
        /// </returns>
        /// <remarks>
        /// The method deserializes the <paramref name="travelFile"/> using <see cref="JsonSerializer"/>
        /// with <see cref="ReferenceHandler.Preserve"/> to handle object reference cycles. 
        /// It then resets primary keys on the imported entities to force insertion of new records,
        /// saves any media files to storage via <c>_document.SaveFile</c>, and finally persists
        /// the graph of entities in "Added" state to the database. Any JSON decoding or format
        /// errors will be caught and returned as a failure result with <see cref="TravelServiceMessage.INVALID_TBIN_FORMAT"/>.
        /// </remarks>
        public async Task<ServiceResult<string>> ImportTravel(byte[] travelFile)
        {
            string payload;
            Trip importedTravel = new Trip();
            TBinModel? binModel = new TBinModel();
            try
            {
                payload = UTF32Encoding.UTF8.GetString(travelFile);

                //Préparer les options pour la désérialisation (avec ReferenceHandler.Preserve)
                var options = new JsonSerializerOptions
                {
                    ReferenceHandler = ReferenceHandler.Preserve,
                    PropertyNameCaseInsensitive = true
                };
                //Désérialiser en un objet (et gérer les références cycliques grâce à Preserve)
                if (options != null && payload != null)
                    binModel = JsonSerializer.Deserialize<TBinModel>(payload, options);

                if (binModel != null)
                    importedTravel = binModel.trip;

            }
            catch (NotSupportedException jsonExc)
            {
                _logger.LogError(jsonExc, "tbin file not compatible");
                return ServiceResult<string>.Failure(TravelServiceMessage.INVALID_TBIN_FORMAT);
            }
            catch (DecoderFallbackException ex)
            {
                _logger.LogError(ex, "tbin file not compatible");
                return ServiceResult<string>.Failure(TravelServiceMessage.INVALID_TBIN_FORMAT);
            }
            if (importedTravel != null && binModel != null && importedTravel.TripBackgroundGuid != null)
            {
                importedTravel.TripId = 0;
                importedTravel.Name = importedTravel.Name + "(Imported)";
                _document.SetMediaType(TypeMedia.Images);
                importedTravel.TripBackgroundGuid = _document.SaveFile(binModel.medias[importedTravel.TripBackgroundGuid.Value]);

                if (importedTravel.Activities != null)
                {
                    foreach (var activity in importedTravel.Activities)
                    {
                        activity.ActivityId = 0;

                        if (activity.ActivityCosts != null)
                        {
                            foreach (var cost in activity.ActivityCosts)
                            {
                                cost.ActivityCostId = 0;
                                cost.ActivityId = 0;
                            }
                        }
                    }
                }
                if (importedTravel.LogBooks != null)
                {
                    foreach (var log in importedTravel.LogBooks)
                    {
                        log.LogBookId = 0;
                        log.TripId = 0;
                    }
                }
                if (importedTravel.Media != null)
                {
                    foreach (var media in importedTravel.Media)
                    {
                        var newGuid = _document.SaveFile(binModel.medias[media.FileGuid]);
                        if (newGuid.HasValue)
                        {
                            media.FileGuid = newGuid.Value;
                        }
                        else
                        {
                            media.FileGuid = Guid.NewGuid();
                        }
                        media.MediaId = 0;
                        media.TripId = 0;
                    }
                }

                // change  context to add 
                try
                {
                    using var context2 = _context.CreateDbContext();

                    // change the contexte in mode Add
                    context2.ChangeTracker.TrackGraph(importedTravel, entry =>
                    {
                        entry.Entry.State = EntityState.Added;
                    });
                    await context2.SaveChangesAsync();
                }
                catch (DbUpdateException ex)
                {
                    _logger.LogError(ex, "Error during importation of a Tbin file ");
                    return ServiceResult<string>.Failure(
                        GlobalServiceMessage.DATABASE_ERROR
                    );
                }
            }
            return ServiceResult<string>.Success(importedTravel?.Name??string.Empty);
        }

        /// <summary>
        /// Asynchronously removes the specified memory files from the given travel record.
        /// </summary>
        /// <param name="selectedMemories">
        /// A collection of <see cref="MemoryFile"/> objects to remove. Must not be null or empty.
        /// </param>
        /// <param name="travelID">
        /// The unique identifier of the travel record. Must be greater than zero.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> if all specified memories were removed successfully (or none were specified).</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.INVALID_TRAVEL_ID"/> if <paramref name="travelID"/> is less than or equal to zero.</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.TRAVEL_NOT_FOUND"/> if no travel record exists for the given <paramref name="travelID"/>.</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.NO_MEDIA"/> if <paramref name="selectedMemories"/> is null or empty.</description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.ERROR_WHEN_REMOVING_FILE"/> if any file fails to delete (note: partial deletions are not rolled back).</description>
        ///   </item>
        /// </list>
        /// </returns>
        public async Task<ServiceResult<bool>> RemoveMemories(
            IEnumerable<MemoryFile>? selectedMemories,
            int travelID)
        {
            if (travelID <= 0)
                return ServiceResult<bool>.Failure(TravelServiceMessage.INVALID_TRAVEL_ID);

            if (selectedMemories == null || !selectedMemories.Any())
                return ServiceResult<bool>.Failure(TravelServiceMessage.NO_MEDIA);

            using var context = _context.CreateDbContext();
            var trip = await context.Trips.FindAsync(travelID);
            if (trip == null)
                return ServiceResult<bool>.Failure(TravelServiceMessage.TRAVEL_NOT_FOUND);

            bool anyFailure = false;
            foreach (var memory in selectedMemories)
            {
                var media = await context.Media.FindAsync(memory.FileID);
                if (media != null)
                {
                    var removed = _document.RemoveFile(media.FileGuid, TypeMedia.Images);
                    if (removed)
                    {
                        context.Media.Remove(media);
                    }
                    else
                    {
                        anyFailure = true;
                    }
                }
            }

            await context.SaveChangesAsync();

            if (anyFailure)
                return ServiceResult<bool>.Failure(TravelServiceMessage.ERROR_WHEN_REMOVING_FILE);

            return ServiceResult<bool>.Success(true);
        }

        /// <summary>
        /// Asynchronously saves a <see cref="Travel"/> instance, including its associated image if provided.
        /// Ensures the operation is atomic by using a database transaction.
        /// </summary>
        /// <param name="travel">
        /// The <see cref="Travel"/> object to save. Must not be null.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> and message <see cref="TravelServiceMessage.TRAVEL_ADDED"/>
        ///       if the travel (and optional image) is saved successfully.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.INVALID_TRAVEL_ID"/> if <paramref name="travel"/> is null.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.UNKNOWN_ERROR"/> if mapping the <see cref="Travel"/> to a <see cref="Trip"/> entity fails.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.ERROR_WHEN_ADDING_FILE"/> if saving the travel's image fails (any saved data will be rolled back).
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        /// <exception cref="Exception">
        /// Propagates any exception encountered during database operations or transaction commit.
        /// </exception>
        public async Task<ServiceResult<bool>> SaveTravel(Travel travel)
        {
            if (travel == null)
                return ServiceResult<bool>.Failure(TravelServiceMessage.INVALID_TRAVEL_ID);

            using var context = _context.CreateDbContext();

            if (!_mapper.TryMap(travel, out Trip trip, _logger))
                return ServiceResult<bool>.Failure(GlobalServiceMessage.UNKNOWN_ERROR);

            //trip.CurrencyCode = travel.currencie;

            Guid? savedFileGuid = null;
            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                //Save The trip without image
                context.Trips.Add(trip);
                await context.SaveChangesAsync();

                //Save Image
                if (travel.image != null)
                {
                    _document.SetMediaType(Commons.TypeMedia.Images);
                    var savedImageGuid = _document.SaveFile(travel.image);
                    if (savedImageGuid == null)
                    {
                        transaction.Rollback();
                        return ServiceResult<bool>.Failure(TravelServiceMessage.ERROR_WHEN_ADDING_FILE);
                    }

                    //Update the trip information
                    trip.TripBackgroundGuid = savedImageGuid;
                    context.Trips.Update(trip);
                    await context.SaveChangesAsync();
                }

                await transaction.CommitAsync();

                return ServiceResult<bool>.Success(true, TravelServiceMessage.TRAVEL_ADDED);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Erreur lors de l'ajout d'un voyage {travelID}", travel.Id);
                await transaction.RollbackAsync();
                //remove orphan file
                if (savedFileGuid.HasValue)
                {
                    _document.RemoveFile(savedFileGuid.Value, Commons.TypeMedia.Images);
                }

                throw;
            }
        }

        /// <summary>
        /// Updates the description of a specific memory file.
        /// </summary>
        /// <param name="memory">
        /// The <see cref="MemoryFile"/> containing the FileID and new Description. Must not be null.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating:
        /// <list type="bullet">
        ///   <item>
        ///     <description><see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> if the description was updated successfully.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.INVALID_MEMORY"/> if <paramref name="memory"/> is null
        ///       or <c>memory.FileID</c> is not valid.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.MEDIA_NOT_FOUND"/> if no media entity matches the given FileID.</description>
        ///   </item>
        ///   <item>
        ///     <description><see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.UNKNOWN_ERROR"/> if an unexpected error occurs during save.</description>
        ///   </item>
        /// </list>
        /// </returns>
        public ServiceResult<bool> UpdateMemory(MemoryFile memory)
        {
            if (memory == null || memory.FileID <= 0)
                return ServiceResult<bool>
                    .Failure(TravelServiceMessage.INVALID_MEMORY);

            using var context = _context.CreateDbContext();

            var media = context.Media
                .FirstOrDefault(m => m.MediaId == memory.FileID);
            if (media == null)
                return ServiceResult<bool>
                    .Failure(TravelServiceMessage.MEDIA_NOT_FOUND);

            media.Description = memory.Description ?? string.Empty;
            context.SaveChanges();

            return ServiceResult<bool>.Success(true);
        }

        /// <summary>
        /// Asynchronously updates an existing <see cref="Travel"/> record, including its associated image if provided.
        /// Ensures atomicity by using a database transaction.
        /// </summary>
        /// <param name="travel">
        /// The <see cref="Travel"/> object containing updated values. Must not be null and must reference an existing record.
        /// </param>
        /// <returns>
        /// A <see cref="ServiceResult{Boolean}"/> indicating:
        /// <list type="bullet">
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Success"/> with <c>true</c> if the update (and optional image replacement) succeeds.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.INVALID_TRAVEL"/> if <paramref name="travel"/> is null.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.UNKNOWN_ERROR"/> if mapping the <see cref="Travel"/> to a <see cref="Trip"/> entity fails.
        ///     </description>
        ///   </item>
        ///   <item>
        ///     <description>
        ///       <see cref="ServiceResult{Boolean}.Failure"/> with
        ///       <see cref="TravelServiceMessage.TRAVEL_NOT_FOUND"/> if no existing record matches <paramref name="travel"/>.<see cref="Travel.Id"/>.
        ///     </description>
        ///   </item>
        /// </list>
        /// </returns>
        /// <exception cref="Exception">
        /// Propagates any exception encountered during database operations or transaction commit; rolls back and cleans up any replaced image file.
        /// </exception>
        public async Task<ServiceResult<bool>> UpdateTravel(Travel travel)
        {
            if (travel == null)
                return ServiceResult<bool>.Failure(TravelServiceMessage.INVALID_TRAVEL);

            using var context = _context.CreateDbContext();

            if (!_mapper.TryMap(travel, out Trip trip, _logger))
                return ServiceResult<bool>.Failure(GlobalServiceMessage.UNKNOWN_ERROR);

            Guid? savedFileGuid = null;

            using var transaction = await context.Database.BeginTransactionAsync();

            try
            {
                var existingTrip = await context.Trips.FindAsync(trip.TripId);
                if (existingTrip == null)
                {
                    return ServiceResult<bool>.Failure(TravelServiceMessage.TRAVEL_NOT_FOUND);
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

                return ServiceResult<bool>.Success(true);
            }
            catch (Exception)
            {
                await transaction.RollbackAsync();

                // Suppression de l'image si elle avait été enregistrée
                if (savedFileGuid.HasValue)
                {
                    _document.RemoveFile(savedFileGuid.Value, Commons.TypeMedia.Images);
                }
                throw;
            }
        }
    }
}
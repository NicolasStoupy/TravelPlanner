using Commons;
using Infrastructure.Documents;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.EntityModels
{
    public partial class TravelPlannerContext
    {

        private readonly DocumentProvider _document;

        public TravelPlannerContext(DbContextOptions<TravelPlannerContext> options, DocumentProvider documentProvider) : base(options)
        {
            _document = documentProvider;
        }

        /// <summary>
        /// Overrides the default SaveChangesAsync method to handle additional logic 
        /// when deleting Media entities from the database.
        /// </summary>
        /// <param name="cancellationToken">A cancellation token that can be used to cancel the asynchronous operation.</param>
        /// <returns>The number of state entries written to the database.</returns>
        /// <remarks>
        /// When a Media entity is marked for deletion, this method ensures that the associated file 
        /// on the file system is also deleted before committing the changes to the database.
        /// </remarks>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            // Media supprimé
            var deletedMedias = ChangeTracker.Entries<Medium>()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in deletedMedias)
            {
                var media = entry.Entity;
                if (Enum.IsDefined(typeof(TypeMedia), media.MediaType))
                {
                    var typeMedia = (TypeMedia)media.MediaType;

                    var succes =_document.RemoveFile(media.FileGuid, typeMedia);
                    if (!succes)
                        throw new Exception("Impossible de supprimer le fichier associé au Trip.");
                }


            }

            // Remove the main trip media  when the trip was deleted by the entity 
            var deletedTrip = ChangeTracker.Entries<Trip>()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            foreach (var trip in deletedTrip)
            {
                var tripImageGUID = trip.Property(x => x.TripBackgroundGuid).OriginalValue;

                if (tripImageGUID.HasValue)
                {
                    var success = _document.RemoveFile(tripImageGUID.Value, TypeMedia.Images);

                    if (!success)
                        throw new Exception("Impossible de supprimer le fichier associé au Trip.");
                }
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

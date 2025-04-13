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
            var deletedMedias = ChangeTracker.Entries<Medium>()
                .Where(e => e.State == EntityState.Deleted)
                .ToList();

            foreach (var entry in deletedMedias)
            {
                var media = entry.Entity;
                if (Enum.IsDefined(typeof(TypeMedia), media.MediaType))
                {
                    var typeMedia = (TypeMedia)media.MediaType;

                    _document.RemoveFile(media.FileGuid, typeMedia);
                }

              
            }

            return await base.SaveChangesAsync(cancellationToken);
        }
    }
}

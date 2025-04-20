using AutoMapper;
using BussinessLogic.Entities;
using Infrastructure.Documents;
using Infrastructure.EntityModels;

namespace BussinessLogic.Mappings.Resolvers
{
    /// <summary>
    /// AutoMapper value resolver that retrieves the associated image file for a trip
    /// using the provided <see cref="DocumentProvider"/> and the trip's background GUID.
    /// </summary>
    public class TravelImageResolver : IValueResolver<Trip, Travel, byte[]?>
    {
        private readonly DocumentProvider _documentProvider;
        /// <summary>
        /// Initializes a new instance of the <see cref="TravelImageResolver"/> class with the specified document provider.
        /// </summary>
        /// <param name="documentProvider">The service used to retrieve image files from storage.</param>
        public TravelImageResolver(DocumentProvider documentProvider)
        {
            _documentProvider = documentProvider;
        }

        /// <summary>
        /// Resolves the image associated with a trip based on its background GUID.
        /// </summary>
        /// <param name="source">The source <see cref="Trip"/> object.</param>
        /// <param name="destination">The destination <see cref="Travel"/> object (not used here).</param>
        /// <param name="destMember">The current value of the destination member (not used).</param>
        /// <param name="context">The AutoMapper resolution context.</param>
        /// <returns>The byte array representing the image, or null if no image is available.</returns>
        public byte[]? Resolve(Trip source, Travel destination, byte[]? destMember, ResolutionContext context)
        {
            return source.TripBackgroundGuid.HasValue
                ? _documentProvider.GetFile(source.TripBackgroundGuid.Value, Commons.TypeMedia.Images)
                : null;
        }
    }


    public class TravelNotesResolver : IValueResolver<Trip, Travel, List<Note>>
    {
        private readonly TravelPlannerContext _context;
        private readonly IMapper _mapper;

        public TravelNotesResolver(TravelPlannerContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<Note> Resolve(Trip source, Travel destination, List<Note> destMember, ResolutionContext context)
        {        

            var logBooks = _context.LogBooks
           .Where(l => l.TripId == source.TripId)
           .ToList();

            return _mapper.Map<List<Note>>(logBooks);
        }


    }
}
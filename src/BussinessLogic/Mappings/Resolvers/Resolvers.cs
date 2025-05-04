using AutoMapper;
using BussinessLogic.Entities;
using Infrastructure.Documents;
using Infrastructure.EntityModels;
using Microsoft.EntityFrameworkCore;

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

    /// <summary>
    /// AutoMapper resolver that maps <see cref="Trip"/> logbook entries to a list of <see cref="Note"/> for a <see cref="Travel"/> object.
    /// </summary>
    public class TravelNotesResolver : IValueResolver<Trip, Travel, List<Note>>
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context;
        private readonly IMapper _mapper;

        public TravelNotesResolver(IDbContextFactory<TravelPlannerContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<Note> Resolve(Trip source, Travel destination, List<Note> destMember, ResolutionContext context)
        {
            var dbcontext = _context.CreateDbContext();
            var logBooks = dbcontext.LogBooks.Where(l => l.TripLogBook == source.TripId);

            return _mapper.Map<List<Note>>(logBooks);
        }
    }

    /// <summary>
    /// AutoMapper resolver that retrieves and maps all <see cref="Activity"/> entities linked to a <see cref="Trip"/>
    /// into a list of <see cref="TravelActivity"/> for the destination <see cref="Travel"/>.
    /// </summary>
    public class TravelActivitiesResolver : IValueResolver<Trip, Travel, List<TravelActivity>>
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context;
        private readonly IMapper _mapper;

        public TravelActivitiesResolver(IDbContextFactory<TravelPlannerContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<TravelActivity> Resolve(Trip source, Travel destination, List<TravelActivity> destMember, ResolutionContext context)
        {
            var dbcontext = _context.CreateDbContext();
            var activities = dbcontext.Activities.Where(a => a.TripId == source.TripId);

            return _mapper.Map<List<TravelActivity>>(activities);
        }
    }

    /// <summary>
    /// AutoMapper resolver that maps all <see cref="Attendee"/> entries related to an <see cref="Activity"/>
    /// into a list of <see cref="Follower"/> for the <see cref="TravelActivity"/>.
    /// </summary>
    public class TravelActivityFollowerResolver : IValueResolver<Activity, TravelActivity, List<Follower>>
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context;
        private readonly IMapper _mapper;

        public TravelActivityFollowerResolver(IDbContextFactory<TravelPlannerContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<Follower> Resolve(Activity source, TravelActivity destination, List<Follower> destMember, ResolutionContext context)
        {
            var dbcontext = _context.CreateDbContext();
            var attendees = dbcontext.Attendees.Where(a => a.ActivityId == source.ActivityId && a.TripId == source.TripId);

            return _mapper.Map<List<Follower>>(attendees);
        }
    }

    /// <summary>
    /// AutoMapper resolver that maps all <see cref="ActivityCost"/> entries for a given <see cref="Activity"/>
    /// into a list of <see cref="Cost"/> for the <see cref="TravelActivity"/> destination.
    /// </summary>
    public class TravelActivityCostResolver : IValueResolver<Activity, TravelActivity, List<Cost>>
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context;
        private readonly IMapper _mapper;

        public TravelActivityCostResolver(IDbContextFactory<TravelPlannerContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<Cost> Resolve(Activity source, TravelActivity destination, List<Cost> destMember, ResolutionContext context)
        {
            var dbcontext = _context.CreateDbContext();
            var attendees = dbcontext.ActivityCosts.Where(c => c.ActivityId == source.ActivityId && c.TripId == source.TripId);

            return _mapper.Map<List<Cost>>(attendees);
        }
    }

    /// <summary>
    /// AutoMapper resolver that maps <see cref="LogBook"/> entries related to a specific <see cref="Activity"/>
    /// into a list of <see cref="Note"/> for the <see cref="TravelActivity"/>.
    /// </summary>
    public class TravelActivityNotesResolver : IValueResolver<Activity, TravelActivity, List<Note>>
    {
        private readonly IDbContextFactory<TravelPlannerContext> _context;
        private readonly IMapper _mapper;

        public TravelActivityNotesResolver(IDbContextFactory<TravelPlannerContext> context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public List<Note> Resolve(Activity source, TravelActivity destination, List<Note> destMember, ResolutionContext context)
        {
            var dbcontext = _context.CreateDbContext();
            var attendees = dbcontext.LogBooks.Where(c => c.ActivityId == source.ActivityId && c.TripId == source.TripId);

            return _mapper.Map<List<Note>>(attendees);
        }
    }
}
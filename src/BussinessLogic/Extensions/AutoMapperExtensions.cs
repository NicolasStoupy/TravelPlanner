using AutoMapper;
using Microsoft.Extensions.Logging;

namespace BussinessLogic.Extensions
{
    public static class AutoMapperExtensions
    {
        /// <summary>
        /// Attempts to map <paramref name="source"/> to <typeparamref name="TDestination"/>.
        /// Returns true on success; false if an AutoMapperMappingException occurs.
        /// </summary>
        public static bool TryMap<TSource, TDestination>(
            this IMapper mapper,
            TSource source,
            out TDestination destination, ILogger logger)
        {
            try
            {
                destination = mapper.Map<TDestination>(source);
                return true;
            }
            catch (AutoMapperMappingException ex)
            {
                logger.LogError(ex, "AutoMapper failed to map from {SourceType} to {DestType}",
                typeof(TSource).Name, typeof(TDestination).Name);
                destination = default!;
                return false;
            }
        }

        /// <summary>
        /// Attempts to map a source object to a destination type using AutoMapper, logging errors on failure.
        /// </summary>
        /// <typeparam name="TSource">The source object type.</typeparam>
        /// <typeparam name="TDestination">The destination object type.</typeparam>
        /// <param name="mapper">The AutoMapper instance used for mapping.</param>
        /// <param name="source">The source object to map from.</param>
        /// <param name="destination">The output destination object if mapping succeeds; default value if mapping fails.</param>
        /// <param name="logger">The logger used to record any mapping errors.</param>
        /// <param name="optsAction">An action to configure mapping options, if needed.</param>
        /// <returns><c>true</c> if mapping succeeds; <c>false</c> if an exception occurs during mapping.</returns>
        public static bool TryMap<TSource, TDestination>(this IMapper mapper, TSource source, out TDestination destination, ILogger logger, Action<IMappingOperationOptions> optsAction)
        {
            try
            {
                destination = mapper.Map<TDestination>(source, optsAction);
                return true;
            }
            catch (AutoMapperMappingException ex)
            {
                logger.LogError(ex,
                    "AutoMapper failed to map from {SourceType} to {DestType}",
                    typeof(TSource).Name, typeof(TDestination).Name);
                destination = default!;
                return false;
            }
        }
    }
}
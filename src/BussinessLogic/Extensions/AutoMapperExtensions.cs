using AutoMapper;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            out TDestination destination,ILogger logger)
        {
            try
            {
                destination = mapper.Map<TDestination>(source);
                return true;
            }
            catch (AutoMapperMappingException ex )
            {
                logger.LogError(ex, "AutoMapper failed to map from {SourceType} to {DestType}",
                typeof(TSource).Name, typeof(TDestination).Name);
                destination = default!;
                return false;
            }
        }

        public static bool TryMap<TSource, TDestination>(
    this IMapper mapper,
    TSource source,
    out TDestination destination,
    ILogger logger,
    Action<IMappingOperationOptions> optsAction)
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

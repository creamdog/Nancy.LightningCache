using System;
using System.Collections.Generic;
using Nancy.Bootstrapper;
using Nancy.LightningCache.CacheKey;
using Nancy.LightningCache.CacheStore;
using Nancy.Routing;

namespace Nancy.LightningCache.Extensions
{
    public static class NancyBootstrapperExtensions
    {
        /// <summary>
        /// Enables Nancy.LightningCache using the supplied parameters
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="routeResolver"> </param>
        /// <param name="pipelines"></param>
        /// <param name="cacheKeyGenerator"></param>
        public static void EnableLightningCache(this INancyBootstrapper bootstrapper, IRouteResolver routeResolver, IPipelines pipelines, ICacheKeyGenerator cacheKeyGenerator)
        {
            LightningCache.Enable(bootstrapper, routeResolver, pipelines, cacheKeyGenerator);
        }

        /// <summary>
        /// Enables Nancy.LightningCache using the supplied parameters and CacheStore type
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="routeResolver"> </param>
        /// <param name="pipelines"></param>
        /// <param name="cacheKeyGenerator"></param>
        /// <param name="cacheStore"> </param>
        public static void EnableLightningCache(this INancyBootstrapper bootstrapper, IRouteResolver routeResolver, IPipelines pipelines, ICacheKeyGenerator cacheKeyGenerator, ICacheStore cacheStore)
        {
            LightningCache.Enable(bootstrapper, routeResolver, pipelines, cacheKeyGenerator, cacheStore);
        }
    }
}

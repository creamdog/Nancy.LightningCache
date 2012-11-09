using System;
using System.Collections.Generic;
using Nancy.Bootstrapper;
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
        /// <param name="varyParams"></param>
        public static void EnableLightningCache(this INancyBootstrapper bootstrapper, IRouteResolver routeResolver, IPipelines pipelines, IEnumerable<string> varyParams)
        {
            LightningCache.Enable(bootstrapper.GetEngine(), routeResolver, pipelines, varyParams);
        }

        /// <summary>
        /// Enables Nancy.LightningCache using the supplied parameters and CacheStore type
        /// </summary>
        /// <param name="bootstrapper"></param>
        /// <param name="routeResolver"> </param>
        /// <param name="pipelines"></param>
        /// <param name="varyParams"></param>
        /// <param name="cacheProviderType"></param>
        public static void EnableLightningCache(this INancyBootstrapper bootstrapper, IRouteResolver routeResolver, IPipelines pipelines, IEnumerable<string> varyParams, Type cacheProviderType)
        {
            LightningCache.Enable(bootstrapper.GetEngine(), routeResolver, pipelines, varyParams, cacheProviderType);
        }
    }
}

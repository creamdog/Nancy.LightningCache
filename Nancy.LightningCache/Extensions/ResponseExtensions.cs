using System;
using Nancy.LightningCache.Projection;

namespace Nancy.LightningCache.Extensions
{
    public static class ResponseExtensions
    {

        /// <summary>
        /// Extension method used to mark this response as cacheable by Nancy.LightningCache
        /// </summary>
        /// <param name="response"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public static Response AsCacheable(this Response response, DateTime expiration)
        {
            return new CacheableResponse(response, expiration);
        }
    }
}

using System;
using System.Globalization;
using Nancy.Responses.Negotiation;

namespace Nancy.LightningCache.Extensions
{
    public static class NegotiatorExtensions
    {
        /// <summary>
        /// Extensions method used to mark this Negotiator as cacheable by Nancy.LightningCache
        /// </summary>
        /// <param name="negotiator"></param>
        /// <param name="expiration"></param>
        /// <returns></returns>
        public static Negotiator AsCacheable(this Negotiator negotiator, DateTime expiration)
        {
            return negotiator.WithHeader("nancy-lightningcache", expiration.ToString(CultureInfo.InvariantCulture));
        }
    }
}

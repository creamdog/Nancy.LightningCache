using System;
using Nancy.LightningCache.Projection;

namespace Nancy.LightningCache.CacheStore
{
    /// <summary>
    /// CacheStore meant to be consumed by Nancy.LightningCache
    /// </summary>
    public interface ICacheStore
    {
        CachedResponse Get(string key);
        void Set(string key, NancyContext context, DateTime absoluteExpiration);
        void Remove(string key);
    }
}

using System;

namespace Nancy.LightningCache.CacheKey
{
    /// <summary>
    /// CacheKeyGenerator meant to be consumed by Nancy.LightningCache
    /// </summary>
    public interface ICacheKeyGenerator
    {
        string Get(Request request);
    }
}

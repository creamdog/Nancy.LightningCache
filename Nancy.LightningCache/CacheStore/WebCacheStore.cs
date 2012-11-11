using System;
using System.Web;
using System.Web.Caching;
using Nancy.LightningCache.Projection;

namespace Nancy.LightningCache.CacheStore
{
    /// <summary>
    /// CacheStore that stores content using the System.Web.Cache
    /// </summary>
    public class WebCacheStore : ICacheStore
    {
        private Cache _cache;

        public CachedResponse Get(string key)
        {
            SetCache();

            if (_cache == null)
                return null;

            var response = _cache.Get(key) as SerializableResponse;

            if (response == null)
                return null;

            return new CachedResponse(response);
        }

        public void Remove(string key)
        {
            SetCache();

            if (_cache == null)
                return;

            _cache.Remove(key);
        }

        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            SetCache();

            if(_cache == null)
                return;

            _cache[key] = new SerializableResponse(context.Response, absoluteExpiration);

        }
        private static readonly object Lock = new object();
        private void SetCache()
        {
            lock(Lock)
            {
                if (HttpContext.Current == null || _cache != null)
                    return;
                _cache = HttpContext.Current.Cache;
            }
        }
    }
}

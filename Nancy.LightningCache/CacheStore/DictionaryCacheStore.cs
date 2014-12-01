using System;
using System.Collections.Concurrent;
using Nancy.LightningCache.Projection;

namespace Nancy.LightningCache.CacheStore
{
    public sealed class DictionaryCacheStore : ICacheStore
    {
        public static ConcurrentDictionary<string, SerializableResponse> Memory { get; private set; }
        public CachedResponse Get(string key)
        {
            return Memory.ContainsKey(key) ? new CachedResponse(Memory[key]) : null;
        }

        static DictionaryCacheStore()
        {
            if (Memory == null) Memory = new ConcurrentDictionary<string, SerializableResponse>();
        }

        public void Remove(string key)
        {
            while (true)
            {
                if (!Memory.ContainsKey(key)) return;
                SerializableResponse d;
                Memory.TryRemove(key, out d);
                if (Memory.ContainsKey(key)) continue;
                break;
            }
        }

        public void Set(string key, NancyContext context, DateTime absoluteExpiration)
        {
            Memory[key] = new SerializableResponse(context.Response, absoluteExpiration);
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;

namespace Nancy.LightningCache.CacheKey
{
    public class DefaultCacheKeyGenerator : ICacheKeyGenerator
    {
        private static string[] _varyParams = new string[0];

        public DefaultCacheKeyGenerator(string[] varyParams)
        {
            _varyParams = varyParams;
        }

        /// <summary>
        /// Generates a cache key from the supplied Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        public string Get(Request request)
        {
            if (request == null || request.Url == null)
                return string.Empty;

            var query = new Dictionary<string, string>();

            if (request.Query is DynamicDictionary)
            {
                var dynDict = (request.Query as DynamicDictionary);
                foreach (var key in dynDict.Keys)
                {
                    query[key] = (string)dynDict[key];
                }
            }

            if (request.Form is DynamicDictionary)
            {
                var dynDict = (request.Form as DynamicDictionary);
                foreach (var key in dynDict.Keys)
                {
                    query[key] = (string)dynDict[key];
                }
            }

            var removeParamKeys = query.Where(a => !_varyParams.Contains(a.Key.Replace("?", "").ToLower())).Select(a => a.Key).ToArray();
            foreach (var removeParamKey in removeParamKeys)
                query.Remove(removeParamKey);

            var url = new Url
            {
                BasePath = request.Url.BasePath,
                HostName = request.Url.HostName,
                Path = request.Url.Path,
                Port = request.Url.Port,
                Query = (query.Count > 0 ? "?" : string.Empty) + string.Join("&", query.Select(a => string.Join("=", a.Key, a.Value))),
                Scheme = request.Url.Scheme,
            };

            return url.ToString();
        }
    }
}

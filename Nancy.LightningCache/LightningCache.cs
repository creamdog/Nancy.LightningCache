using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Nancy.Bootstrapper;
using Nancy.LightningCache.CacheStore;
using Nancy.LightningCache.Projection;
using Nancy.Routing;

namespace Nancy.LightningCache
{
    /// <summary>
    /// Asynchronous cache for Nancy 
    /// </summary>
    public class LightningCache
    {
        private static readonly string NO_REQUEST_CACHE_KEY = "_lightningCacheDisabled";

        private static string[] _varyParams = new string[0];
        private static ICacheStore _cacheStore;
        
        private static bool _enabled;

        private static INancyEngine _nancyEngine;
        private static INancyEngine NancyEngine
        {
            get
            {
                _nancyEngine = _nancyEngine ?? _nancyBootstrapper.GetEngine();
                return _nancyEngine;
            }
        }
        private static IRouteResolver _routeResolver;

        private static INancyBootstrapper _nancyBootstrapper;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipelines"></param>
        /// <param name="varyParams"></param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver, IPipelines pipelines, IEnumerable<string> varyParams)
        {
            if (_enabled)
                return;
            _enabled = true;
            _varyParams = varyParams.Select(key => key.ToLower().Trim()).ToArray();
            _cacheStore = new WebCacheStore();
            _nancyBootstrapper = nancyBootstrapper;
            _routeResolver = routeResolver;
            pipelines.BeforeRequest.AddItemToStartOfPipeline(CheckCache);
            pipelines.AfterRequest.AddItemToEndOfPipeline(SetCache);
            
        }


        /// <summary>
        /// 
        /// </summary>
        /// <param name="nancyBootstrapper"></param>
        /// <param name="routeResolver"></param>
        /// <param name="pipeline"></param>
        /// <param name="varyParams"></param>
        /// <param name="cacheStore"></param>
        public static void Enable(INancyBootstrapper nancyBootstrapper, IRouteResolver routeResolver, IPipelines pipeline, IEnumerable<string> varyParams, ICacheStore cacheStore)
        {
            if (_enabled)
                return;
            _enabled = true;
            _varyParams = varyParams.Select(key => key.ToLower().Trim()).ToArray();
            _cacheStore = cacheStore;
            _nancyBootstrapper = nancyBootstrapper;
            _routeResolver = routeResolver;
            pipeline.BeforeRequest.AddItemToStartOfPipeline(CheckCache);
            pipeline.AfterRequest.AddItemToEndOfPipeline(SetCache);
        }

        /// <summary>
        /// Invokes pre-requirements such as authentication and stuff for the supplied context
        /// reference: https://github.com/NancyFx/Nancy/blob/master/src/Nancy/Routing/DefaultRequestDispatcher.cs
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static Response InvokePreRequirements(NancyContext context)
        {
            var resolution = _routeResolver.Resolve(context);
            var preRequirements = resolution.Before;
            return preRequirements.Invoke(context);
        }

        /// <summary>
        /// checks current cachestore for a cached response and returns it
        /// </summary>
        /// <param name="context"></param>
        /// <returns>cached response or null</returns>
        private static Response CheckCache(NancyContext context)
        {
            if (context.Request.Query is DynamicDictionary)
                if ((context.Request.Query as DynamicDictionary).ContainsKey(NO_REQUEST_CACHE_KEY))
                    return null;

            var key = GetCacheKey(context.Request);

            if (string.IsNullOrEmpty(key))
                return null;

            var response = _cacheStore.Get(key);
            
            if (response == null)
                return null;

            if(response.Expiration < DateTime.Now)
            {
                var t = new Thread(HandleRequestAsync);
                t.Start(context.Request);
            }

            //make damn sure the pre-requirements are met before returning a cached response
            var preResponse = InvokePreRequirements(context);
            if (preResponse != null)
                return preResponse;

            return response;
        }

        /// <summary>
        /// caches response before it is sent to client if it is a CacheableResponse or if the NegotationContext has the nancy-lightningcache header set.
        /// </summary>
        /// <param name="context"></param>
        private static void SetCache(NancyContext context)
        {
            if (context.Response is CachedResponse)
                return;

            var key = GetCacheKey(context.Request);

            if (string.IsNullOrEmpty(key))
                return;

            if (context.Response is CacheableResponse)
            {
                if(context.Response.StatusCode != HttpStatusCode.OK)
                {
                    _cacheStore.Remove(key);
                    return;
                }
                _cacheStore.Set(key, context, (context.Response as CacheableResponse).Expiration);
            }
            else if (context.NegotiationContext != null && context.NegotiationContext.Headers != null && context.NegotiationContext.Headers.ContainsKey("nancy-lightningcache"))
            {
                if (context.Response.StatusCode != HttpStatusCode.OK)
                {
                    _cacheStore.Remove(key);
                    return;
                }
                var expiration = DateTime.Parse(context.NegotiationContext.Headers["nancy-lightningcache"]);
                context.NegotiationContext.Headers.Remove("nancy-lightningcache");
                _cacheStore.Set(key, context, expiration);
            }
        }

        private static readonly List<string> RequestSyncKeys = new List<string>();
        private static readonly object Lock = new object();
        /// <summary>
        /// used to asynchronously cache Nancy Requests
        /// </summary>
        /// <param name="context"></param>
        private static void HandleRequestAsync(object context)
        {
            lock(Lock)
            {
                var request = context as Request;

                if (request == null)
                    return;

                var key = GetCacheKey(request);

                if(string.IsNullOrEmpty(key))
                    return;

                try
                {
                    if (RequestSyncKeys.Contains(key))
                        return;

                    RequestSyncKeys.Add(key);

                    request.Query[NO_REQUEST_CACHE_KEY] = NO_REQUEST_CACHE_KEY;

                    var context2 = NancyEngine.HandleRequest(request);

                    if(context2.Response.StatusCode != HttpStatusCode.OK)
                        _cacheStore.Remove(key);
                }
                catch (Exception)
                {
                    _cacheStore.Remove(key);
                }
                finally
                {
                    RequestSyncKeys.Remove(key);
                }
            }
        }


        /// <summary>
        /// Generates a cache key from the supplied Request
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        private static string GetCacheKey(Request request)
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
                Fragment = request.Url.Fragment,
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

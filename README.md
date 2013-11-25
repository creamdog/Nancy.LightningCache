Nancy.LightningCache
====================

Delivers dynamic content with the speed of static content using asyncronous caching. Nancy.LightningCache will allways deliver cached responses if they exist and update asynchronously if needed.

##Installation

Install via nuget https://nuget.org/packages/Nancy.LightningCache

```
PM> Install-Package Nancy.LightningCache
```

Or build from source and drop Nancy.LightningCache.dll into your solution

##Example usage
the following example is using the default "System.Web.Cache" CacheStore
###1. Add to your bootstrapper

```c#
using Nancy.LightningCache.Extensions;
using Nancy.Routing;

namespace Asp.Net.Example
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            /*enable lightningcache, vary by url params id,query,take and skip*/
            this.EnableLightningCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "id", "query", "take", "skip" });
        }
    }
}
```

###2. Enable caching by adding "AsCacheable" to any of your routes
```c#
using System;
using Nancy;
using Nancy.LightningCache.Extensions;

namespace Asp.Net.Example
{
    public class ExampleModule : NancyModule
    {
        public ExampleModule()
        {
            Get["/"] = _ =>
            {
                /*cache view*/
                return View["hello.html"].AsCacheable(DateTime.Now.AddSeconds(30));
            };

            Get["/CachedResponse"] = _ =>
            {
                /*cache response*/
                return Response.AsText("hello").AsCacheable(DateTime.Now.AddSeconds(30));
            };
        }
    }
}
```

##Example usage of the DiskCacheStore
If your application does not have access to "System.Web.Cache" as in running in self hosting mode you can use the DiskCacheStore to enable caching thought LightningCache.
```c#
using Nancy.LightningCache.CacheStore;
using Nancy.LightningCache.Extensions;
using Nancy.Routing;

namespace Asp.Net.Example
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            /*enable lightningcache using the DiskCacheStore, vary by url params id,query,take and skip*/
            this.EnableLightningCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "id", "query", "take", "skip" }, new DiskCacheStore("c:/tmp/cache"));
        }
    }
}
```

##Example definining your own cache key generation using ICacheKeyGenerator
If your application does not have access to "System.Web.Cache" as in running in self hosting mode you can use the DiskCacheStore to enable caching thought LightningCache.
```c#
using System;
using System.Text;
using Nancy;
using Nancy.LightningCache.Extensions;
using Nancy.Routing;

namespace WebApplication
{
    public class Bootstrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void ApplicationStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines)
        {
            base.ApplicationStartup(container, pipelines);
            this.EnableLightningCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new UrlHashKeyGenerator());
        }

        public class UrlHashKeyGenerator : Nancy.LightningCache.CacheKey.ICacheKeyGenerator
        {
            public string Get(Request request)
            {
               using(var md5 = new System.Security.Cryptography.MD5CryptoServiceProvider())
               {
                   var hash = md5.ComputeHash(Encoding.UTF8.GetBytes(request.Url.ToString()));
                   return Convert.ToBase64String(hash);
               }
            }
        }
    }
}
```

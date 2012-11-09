Nancy.LightningCache
====================

Delivers dynamic content with the speed of static content using asyncronous caching. Nancy.LightningCache will allways deliver cached responses if they exist and update asynchronously if needed.

##Example usage
the following example is using the default "System.Web.Cache" CacheStore
###1. Add to your bootstrapper
    using Nancy.LightningCache.Extensions;
    using Nancy.Routing;
    
    namespace Asp.Net.Example
    {
        public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
        {
            protected override void RequestStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, Nancy.NancyContext context)
            {
                /*enable lightningcache, vary by url params id,query,take and skip*/
                this.EnableLightningCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new []{"id","query","take","skip"});
                base.RequestStartup(container, pipelines, context);
            }
        }
    }

###2. Enable caching by adding "AsCacheable" to any of your routes
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
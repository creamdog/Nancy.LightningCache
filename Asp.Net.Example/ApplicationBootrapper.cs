using Nancy.LightningCache.CacheStore;
using Nancy.LightningCache.Extensions;
using Nancy.Routing;

namespace Asp.Net.Example
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void RequestStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, Nancy.NancyContext context)
        {
            /*enable lightningcache, vary by url params id,query,take and skip*/
            //this.EnableLightningCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new []{"id","query","take","skip"});

            /*enable lightningcache using the DiskCacheStore, vary by url params id,query,take and skip*/
            this.EnableLightningCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new[] { "id", "query", "take", "skip" }, new DiskCacheStore("c:/tmp/cache"));

            base.RequestStartup(container, pipelines, context);
        }
    }
}
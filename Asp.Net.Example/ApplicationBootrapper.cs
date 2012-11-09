using Nancy.LightningCache.Extensions;
using Nancy.Routing;

namespace Asp.Net.Example
{
    public class ApplicationBootrapper : Nancy.DefaultNancyBootstrapper
    {
        protected override void RequestStartup(Nancy.TinyIoc.TinyIoCContainer container, Nancy.Bootstrapper.IPipelines pipelines, Nancy.NancyContext context)
        {
            this.EnableLightningCache(container.Resolve<IRouteResolver>(), ApplicationPipelines, new []{"id","query","take","skip"});
            base.RequestStartup(container, pipelines, context);
        }
    }
}
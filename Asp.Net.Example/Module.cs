using System;
using System.Globalization;
using Nancy;
using Nancy.LightningCache.Extensions;

namespace Asp.Net.Example
{
    public class Module : NancyModule
    {
        public Module()
        {
            Get["/"] = _ =>
            {
                return View["TestView.html", new { Hello = DateTime.Now.ToString(CultureInfo.InvariantCulture)}].AsCacheable(DateTime.Now.AddSeconds(1));
            };

            Get["/CachedResponse"] = _ =>
            {
                return Response.AsText(@"
                this is a cached response: "+DateTime.Now.ToString(CultureInfo.InvariantCulture)+@"
                ").AsCacheable(DateTime.Now.AddSeconds(1));
            };
        }


    }
}
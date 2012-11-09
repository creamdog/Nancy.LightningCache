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
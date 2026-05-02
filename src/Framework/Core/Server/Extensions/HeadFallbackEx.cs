using System.IO;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Http;

namespace Crudspa.Framework.Core.Server.Extensions;

public static class HeadFallbackEx
{
    public static IApplicationBuilder UseHeadFallback(this IApplicationBuilder app)
    {
        return app.Use(async (context, next) =>
        {
            if (!HttpMethods.IsHead(context.Request.Method))
            {
                await next();
                return;
            }

            var originalMethod = context.Request.Method;
            var originalBody = context.Response.Body;

            context.Request.Method = HttpMethods.Get;
            context.Response.Body = Stream.Null;

            try
            {
                await next();
            }
            finally
            {
                context.Request.Method = originalMethod;
                context.Response.Body = originalBody;
            }
        });
    }
}
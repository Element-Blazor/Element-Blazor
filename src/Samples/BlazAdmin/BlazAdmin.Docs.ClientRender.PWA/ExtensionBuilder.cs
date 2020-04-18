using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace Blazui.Admin.Docs.ClientRender.PWA
{
    public static class ExtensionBuilder
    {
        public static IServiceCollection AddBlazui.Admin(this IServiceCollection services)
        {
            return services;
        }

        public static IApplicationBuilder UseBlazui.Admin(this IApplicationBuilder app)
        {
            app.UseAuthorization();
            app.UseAuthentication();
            return app;
        }
    }
}

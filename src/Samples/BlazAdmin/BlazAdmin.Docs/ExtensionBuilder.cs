using BlazAdmin.ServerRender;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazAdmin.Docs
{
    public static class ExtensionBuilder
    {
        public static IServiceCollection AddBlazAdmin(this IServiceCollection services)
        {
            services.AddBlazAdmin<DocsDbContext>();
            return services;
        }

        public static IApplicationBuilder UseBlazAdmin(this IApplicationBuilder app)
        {
            app.UseAuthorization();
            app.UseAuthentication();
            return app;
        }
    }
}

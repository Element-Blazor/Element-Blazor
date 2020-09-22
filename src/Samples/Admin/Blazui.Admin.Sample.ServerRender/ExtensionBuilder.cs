using Blazui.Admin.Abstract;
using Blazui.Admin.ServerRender;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using Element;

namespace Blazui.Admin.Sample.ServerRender
{
    public static class ExtensionBuilder
    {
        public static  IServiceCollection  AddAdmin<TUserService>(this IServiceCollection services)
            where TUserService : class, IUserService
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpClient();
            services.AddBlazuiServices();
            services.AddSingleton<RouteService>();
            services.AddScoped<IUserService, TUserService>();
            services.AddAdmin<DocsDbContext>();
            return services;
        }

        public static IApplicationBuilder UseAdmin(this IApplicationBuilder app)
        {
            app.UseAuthorization();
            app.UseAuthentication();
            return app;
        }
    }
}

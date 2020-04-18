using Blazui.Admin.Abstract;
using Blazui.Admin.ServerRender;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using Blazui.Component;

namespace Blazui.Admin.Sample.ServerRender
{
    public static class ExtensionBuilder
    {
        public static async System.Threading.Tasks.Task<IServiceCollection> AddAdminAsync<TUserService>(this IServiceCollection services)
            where TUserService : class, IUserService
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpClient();
            await services.AddBlazuiServicesAsync();
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

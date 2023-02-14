using Element.Admin.Abstract;
using Element.Admin.ServerRender;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using System;
using Element;

namespace Element.Admin.Sample.ServerRender
{
    public static class ExtensionBuilder
    {
        public static async System.Threading.Tasks.Task<IServiceCollection> AddAdminAsync<TUserService>(this IServiceCollection services)
            where TUserService : class, IUserService
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpClient();
            services.AddBlazuiServices();
            services.AddSingleton<RouteService>();
            services.AddScoped<IUserService, TUserService>();
            services.AddAdmin<DocsDbContext, IdentityUser>();
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

using BlazAdmin.Abstract;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace BlazAdmin.ServerRender
{
    public static class ExtensionBuilder
    {
        public static IServiceCollection AddBlazAdmin<TDbContext>(this IServiceCollection services)
            where TDbContext : IdentityDbContext
        {
            services.AddBlazAdmin<IdentityUser, UserService, TDbContext>(null);
            return services;
        }

        public static IServiceCollection AddBlazAdmin<TUser, TUserService, TDbContext>(this IServiceCollection services, Action<IdentityOptions> optionConfigure)
            where TUser : IdentityUser
            where TDbContext : IdentityDbContext
            where TUserService : class, IUserService
        {
            services.AddControllers();
            services.AddBlazAdminCore<TUserService>();
            services.AddAuthentication(o =>
            {
                o.DefaultScheme = IdentityConstants.ApplicationScheme;
                o.DefaultSignInScheme = IdentityConstants.ExternalScheme;
            })
            .AddIdentityCookies();
            var builder = services.AddIdentityCore<TUser>(o =>
              {
                  o.Stores.MaxLengthForKeys = 128;
              }).AddRoles<IdentityRole>()
              .AddSignInManager()
              .AddDefaultTokenProviders()
              .AddEntityFrameworkStores<TDbContext>();

            if (optionConfigure != null)
            {
                services.Configure(optionConfigure);
            }
            else
            {
                services.Configure<IdentityOptions>(options =>
                {
                    // Default Password settings.
                    options.Password.RequireDigit = false;
                    options.Password.RequireLowercase = false;
                    options.Password.RequireNonAlphanumeric = false;
                    options.Password.RequireUppercase = false;
                    options.Password.RequiredLength = 6;
                    options.Password.RequiredUniqueChars = 1;
                });
            }
            services.AddScoped<AuthenticationStateProvider, RevalidatingIdentityAuthenticationStateProvider<IdentityUser>>();
            return services;
        }
        public static IApplicationBuilder UseBlazAdminServer(this IApplicationBuilder applicationBuilder)
        {
            //applicationBuilder.UseBlazAdminServerCore();
            //applicationBuilder.UseSwagger();
            //applicationBuilder.UseSwaggerUI(c =>
            //{
            //    c.SwaggerEndpoint("/swagger/v1/swagger.json", "BlazAdmin Api V1");
            //    c.RoutePrefix = string.Empty;
            //});
            return applicationBuilder;
        }
    }
}

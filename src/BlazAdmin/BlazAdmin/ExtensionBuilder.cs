using BlazAdmin.Abstract;
using Blazui.Component;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace BlazAdmin
{
    public static class ExtensionBuilder
    {
        public static IServiceCollection AddBlazAdminCore<TUserService>(this IServiceCollection services)
            where TUserService : class, IUserService
        {
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpClient();
            services.AddBlazuiServices();
            services.AddSingleton<RouteService>();
            services.AddScoped<IUserService, TUserService>();
            return services;
        }

        //public static IApplicationBuilder UseBlazAdminCore(this IApplicationBuilder builder)
        //{
        //    builder.UseMiddleware<ReverseProxyMiddleware>();
        //    return builder;
        //}
    }
}

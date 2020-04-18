using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;

namespace Blazui.Admin.ClientRender
{
    public static class ExtensionBuilder
    {
        public static IServiceCollection AddBlazui.Admin(this IServiceCollection services)
        {

            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
            services.AddScoped<SignOutSessionStateManager>();
        }

        public static IApplicationBuilder UseBlazui.Admin(this IApplicationBuilder builder)
        {
            builder.ApplicationServices.AddApiAuthorization();
            return builder;
        }
    }
}

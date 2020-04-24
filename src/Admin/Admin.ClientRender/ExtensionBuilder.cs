//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.AspNetCore.Components.WebAssembly.Authentication;
//using System;
//using System.Collections.Generic;
//using System.Text;
//using Microsoft.AspNetCore.Builder;
//using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

//namespace Blazui.Admin.ClientRender
//{
//    public static class ExtensionBuilder
//    {
//        public static IServiceCollection AddAdmin(this WebAssemblyHostBuilder builder)
//        {
//            services.AddScoped<AuthenticationStateProvider, ServerAuthenticationStateProvider>();
//            services.AddScoped<SignOutSessionStateManager>();
//        }

//        public static IApplicationBuilder UseAdmin(this IApplicationBuilder builder)
//        {
//            builder.ApplicationServices.AddApiAuthorization();
//            return builder;
//        }
//    }
//}

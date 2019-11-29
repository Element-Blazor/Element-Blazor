using Blazui.Component.Dom;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public static class Extension
    {
        public static IServiceCollection AddBlazuiServices(this IServiceCollection services)
        {
            services.AddScoped<Document>();
            services.AddScoped<MessageService>();
            services.AddScoped<LoadingService>();
            services.AddScoped<DialogService>();
            services.AddScoped<PopupService>();
            services.AddScoped<MessageBox>();
            services.AddHttpClient();
            return services;
        }
    }
}

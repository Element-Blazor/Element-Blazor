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
            services.AddSingleton<Document>();
            return services;
        }
    }
}

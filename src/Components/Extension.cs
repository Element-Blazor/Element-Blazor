using Blazui.Component.Lang;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public static class Extension
    {
        /// <summary>
        /// 添加 Blazui 相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <returns></returns>
        public static IServiceCollection AddBlazuiServices(this IServiceCollection services)
        {
            services.AddScoped<Document>();
            services.AddScoped<MessageService>();
            services.AddSingleton<LoadingService>();
            services.AddScoped<DialogService>();
            services.AddScoped<PopupService>();
            services.AddScoped<MessageBox>();
            services.AddTransient<BLang>();
            return services;
        }
    }
}

using Blazui.Component.Lang;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public static class Extension
    {
        /// <summary>
        /// 添加 Blazui 相关服务
        /// </summary>
        /// <param name="services"></param>
        /// <param name="lang">默认语言</param>
        /// <returns></returns>
        public static async Task<IServiceCollection> AddBlazuiServicesAsync(this IServiceCollection services, string lang = "zh-CN")
        {
            services.AddScoped<Document>();
            services.AddScoped<MessageService>();
            services.AddSingleton<LoadingService>();
            services.AddScoped<DialogService>();
            services.AddScoped<PopupService>();
            services.AddScoped<MessageBox>();
            services.AddHttpClient();
            var httpClient = services.BuildServiceProvider().GetRequiredService<HttpClient>();
            Console.WriteLine(httpClient);
            var configuration = await SetLocaleAsync(httpClient, lang);
            services.AddSingleton(provider =>
            {
                Console.WriteLine(httpClient);
                return new BLang(configuration, lang, SetLocaleAsync, httpClient);
            });
            return services;
        }

        private static async Task<IConfiguration> SetLocaleAsync(HttpClient httpClient, string locale)
        {
            var path = new StringBuilder();
            path.Append(locale);
            path.Append(".Json");
            var type = Assembly.LoadFrom("Microsoft.JSInterop.WebAssembly.dll").GetType("Microsoft.JSInterop.WebAssembly.WebAssemblyJSRuntime");
            if (type == null)
            {
                Console.WriteLine("Current Host Is Server");
                return new ConfigurationBuilder()
                     .Add(new JsonConfigurationSource { Path = path.ToString(), Optional = false, ReloadOnChange = true })
                     .Build();
            }
            Console.WriteLine("Current Host Is WebAssembly");
            if (httpClient == null)
            {
                throw new Exception("请添加 HttpClient 依赖");
            }
            var jsonResponse = await (await httpClient.GetAsync(path.ToString())).Content.ReadAsStreamAsync();
            return new ConfigurationBuilder()
                .AddJsonStream(jsonResponse)
                .Build();
        }
    }
}

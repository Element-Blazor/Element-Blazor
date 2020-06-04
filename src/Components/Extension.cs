using Blazui.Component.ControlRender;
using Blazui.Component.ControlRenders;
using Blazui.Component.Lang;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
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
        public static async Task<IServiceCollection> AddBlazuiServicesAsync(this IServiceCollection services, string lang = null)
        {
            services.AddSingleton<FormFieldControlMap>();
            services.AddScoped<Document>();
            services.AddScoped<MessageService>();
            services.AddSingleton<LoadingService>();
            services.AddScoped<DialogService>();
            services.AddScoped<PopupService>();
            services.AddScoped<MessageBox>();
            services.AddScoped<IInputRender, EmptyRender>();
            services.AddScoped<ISelectRender, SelectRender>();
            services.AddScoped<ISwitchRender, SwitchRender>();
            services.AddScoped<IDatePickerRender, EmptyRender>();
            var httpClient = services.BuildServiceProvider().GetRequiredService<HttpClient>();
            var configuration = await SetLocaleAsync(httpClient, lang);
            services.AddSingleton(provider =>
            {
                return new BLang(configuration, lang, SetLocaleAsync, httpClient);
            });
            return services;
        }

        private static async Task<IConfiguration> SetLocaleAsync(HttpClient httpClient, string locale)
        {
            if (string.IsNullOrWhiteSpace(locale))
            {
                return null;
            }
            Type type = null;
            try
            {
                type = Assembly.LoadFrom("Microsoft.JSInterop.WebAssembly.dll").GetType("Microsoft.JSInterop.WebAssembly.WebAssemblyJSRuntime");
            }
            catch (FileNotFoundException)
            {

            }
            if (type == null)
            {
                Console.WriteLine("Current Host Is Server");
                var filePath = $"{Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wwwroot", "lang", locale + ".json")}";
                if (!File.Exists(filePath))
                {
                    filePath = $"{Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "wwwroot", "_content", "Blazui.Component", "lang", locale + ".json")}";
                }
                if (!File.Exists(filePath))
                {
                    throw new Exception(filePath);
                }
                return new ConfigurationBuilder()
                    .AddJsonFile(filePath)
                     .Build();
            }
            Console.WriteLine("Current Host Is WebAssembly");
            if (httpClient == null)
            {
                throw new Exception("请添加 HttpClient 依赖");
            }
            var path = new StringBuilder();
            path.Append("/_content/Blazui.Component/lang/");
            path.Append(locale);
            path.Append(".json");
            var jsonResponse = await (await httpClient.GetAsync(path.ToString())).Content.ReadAsStreamAsync();
            return new ConfigurationBuilder()
                .AddJsonStream(jsonResponse)
                .Build();
        }
    }
}

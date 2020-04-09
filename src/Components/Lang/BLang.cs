using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;

namespace Blazui.Component.Lang
{
    /// <summary>
    /// Lang
    /// </summary>
    public class BLang
    {
        public BLang(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }
        /// <summary>
        /// 一组键/值应用程序配置属性
        /// </summary>
        IConfiguration Configuration { get; set; }
        private HttpClient httpClient;
        private string langLocale = "/zh-CN";

        public string LangLocale
        {
            get
            {
                return langLocale;
            }
            set
            {
                langLocale = value;
                InitBLangBase();
            }
        }

        public BLang()
        {
            InitBLangBase();
        }

        public void InitBLangBase()
        {
            var path = new StringBuilder();
            path.Append(LangLocale);
            path.Append(".Json");
            if (Type.GetType($"Microsoft.AspNetCore.Components.WebAssembly.Hosting.WebAssemblyHostBuilder, Microsoft.AspNetCore.Components.WebAssembly.Hosting") == null)
            {
                Console.WriteLine("Current Host Is WebAssembly");
                if (httpClient == null)
                {
                    throw new Exception("请添加 HttpClient 依赖");
                }
                var jsonResponse = httpClient.GetAsync(path.ToString()).Result.Content.ReadAsStreamAsync().Result;
                Configuration = new ConfigurationBuilder()
                    .AddJsonStream(jsonResponse)
                    .Build();
                return;
            }
            Console.WriteLine("Current Host Is Server");
            Configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = path.ToString(), Optional = false, ReloadOnChange = true })
                .Build();
        }

        public string T(string sections)
        {
            return Configuration?[sections];
        }
    }
}

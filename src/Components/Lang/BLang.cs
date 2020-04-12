using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System;
using System.Net.Http;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component.Lang
{
    /// <summary>
    /// Lang
    /// </summary>
    public class BLang
    {
        public BLang(IConfiguration configuration, string locale, Func<HttpClient, string, Task<IConfiguration>> refreshConfiguration, HttpClient httpClient)
        {
            this.configuration = configuration;
            this.CurrentLang = locale;
            this.refreshConfiguration = refreshConfiguration;
            this.httpClient = httpClient;
        }
        /// <summary>
        /// 一组键/值应用程序配置属性
        /// </summary>
        private IConfiguration configuration { get; set; }
        private HttpClient httpClient;

        /// <summary>
        /// 当前语言，默认中文
        /// </summary>
        public string CurrentLang { get; private set; }
        private Func<HttpClient, string, Task<IConfiguration>> refreshConfiguration { get; set; }

        /// <summary>
        /// 设置语言文件的文件名
        /// </summary>
        /// <param name="lang"></param>
        /// <returns></returns>
        public async Task SetLangAsync(string lang)
        {
            CurrentLang = lang;
            configuration = await refreshConfiguration(httpClient, lang);
        }

        public string T(string sections)
        {
            return configuration?[sections];
        }
    }
}

using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace Blazui.Component.Lang
{
    /// <summary>
    /// Lang
    /// </summary>
    public class BLang
    {
        public static async Task<BLang> CreateBLangAsync(string locale, Func<HttpClient, string, Task<IConfiguration>> refreshConfiguration, HttpClient httpClient)
        {
            if (string.IsNullOrWhiteSpace(locale))
            {
                locale = DefaultLang;
            }

            var bLang = new BLang(locale, refreshConfiguration, httpClient);
            await bLang.SetLangAsync(locale);
            return bLang;
        }

        private BLang(string locale, Func<HttpClient, string, Task<IConfiguration>> refreshConfiguration, HttpClient httpClient)
        {
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
        /// 默认语言，中文
        /// </summary>
        public const string DefaultLang = "zh-CN";

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

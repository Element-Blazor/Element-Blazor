using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Json;
using System.Text;

namespace Blazui.Component.Lang
{
    /// <summary>
    /// Lang
    /// </summary>
    public class BLangBase : ILang
    {
        /// <summary>
        /// 一组键/值应用程序配置属性
        /// </summary>
        IConfiguration Configuration { get; set; }
        private string langLocale = "/zh-CN";

        public string LangLocale {
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

        public BLangBase()
        {
            InitBLangBase();
        }

        public void InitBLangBase()
        {
            var Path = new StringBuilder();
            Path.Append(LangLocale);
            Path.Append(".Json");
            Configuration = new ConfigurationBuilder()
                .Add(new JsonConfigurationSource { Path = Path.ToString(), Optional = false, ReloadOnChange = true })
                .Build();
        }

        public string T(string sections)
        {
            return Configuration?[sections];
        }
    }
}

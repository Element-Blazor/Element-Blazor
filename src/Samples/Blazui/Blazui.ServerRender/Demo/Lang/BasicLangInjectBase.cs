using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Lang
{
    public class BasicLangInjectBase : ComponentBase
    {
        [Inject]
        protected Component.Lang.BLang Lang { get; set; }

        protected override Task OnInitializedAsync()
        {
            Lang.LangLocale = "en-US";
            return base.OnInitializedAsync();
        }

        public async Task SetEnLang(MouseEventArgs eventArgs)
        {
            Lang.LangLocale = "en-US";
        }

        public async Task SetCnLang(MouseEventArgs eventArgs)
        {
            Lang.LangLocale = "zh-CN";
        }
    }
}

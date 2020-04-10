using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ClientRender.Demo.Lang
{
    public class BasicLangBaseInject : ComponentBase
    {
        [Inject]
        public Component.Lang.BLang Lang { get; set; }

        protected override Task OnInitializedAsync()
        {
            Lang.CurrentLang = "en-US";
            return base.OnInitializedAsync();
        }

        public async Task SetEnLang(MouseEventArgs eventArgs)
        {
            Lang.CurrentLang = "en-US";
        }

        public async Task SetCnLang(MouseEventArgs eventArgs)
        {
            Lang.CurrentLang = "zh-CN";
        }
    }
}

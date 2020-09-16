using Blazui.ClientRender.Pages;
using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ClientRender.Demo.Lang
{
    public class BasicLangBase : BComponentBase
    {
        [Inject]
        public Component.Lang.BLang Lang { get; set; }
        public async Task SetEnLang(MouseEventArgs eventArgs)
        {
            await Lang.SetLangAsync("en-US");
            MarkAsRequireRender();
        }

        public async Task SetCnLang(MouseEventArgs eventArgs)
        {
          
            await Lang.SetLangAsync("zh-CN");
            MarkAsRequireRender();
        }
    }
}

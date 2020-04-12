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
        public async Task SetEnLang(MouseEventArgs eventArgs)
        {
            Lang.CurrentLang = "en-US";
            MarkAsRequireRender();
        }

        public async Task SetCnLang(MouseEventArgs eventArgs)
        {
            Lang.CurrentLang = "zh-CN";
            MarkAsRequireRender();
        }
    }
}

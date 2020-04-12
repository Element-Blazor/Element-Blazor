using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Lang
{
    public class BasicLangBase : BComponentBase
    {
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

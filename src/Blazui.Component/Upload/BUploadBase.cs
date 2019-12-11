using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Upload
{
    public class BUploadBase : BComponentBase
    {
        internal ElementReference Input { get; set; }
        internal async Task SelectFileAsync(MouseEventArgs args)
        {
            await Input.Dom(JSRuntime).ClickAsync();
        }

        internal async Task ReadFileAsync()
        {
            var infos = await Input.Dom(JSRuntime).ReadFileAsync("/api/test/upload");
        }
    }
}

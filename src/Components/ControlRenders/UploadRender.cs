using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component.ControlRenders
{
    internal class UploadRender : IUploadRender
    {
        public object Data { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public void Render(RenderTreeBuilder renderTreeBuilder, FormItemConfig config)
        {
            renderTreeBuilder.OpenComponent<BUpload>(0);
            renderTreeBuilder.AddAttribute(1,nameof(BUpload.Url),config.RequiredMessage)

            renderTreeBuilder.CloseComponent();
        }
    }
}

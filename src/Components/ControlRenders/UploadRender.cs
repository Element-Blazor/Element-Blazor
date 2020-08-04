using Blazui.Component.ControlConfigs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Blazui.Component.ControlRenders
{
    internal class UploadRender : IUploadRender
    {
        public void Render(RenderTreeBuilder renderTreeBuilder, RenderConfig config)
        {
            var uploadConfig = (UploadAttribute)config.ControlAttribute;
            renderTreeBuilder.OpenComponent<BUpload>(0);
            renderTreeBuilder.AddAttribute(1, nameof(BUpload.Url), uploadConfig.Url);
            renderTreeBuilder.AddAttribute(2, nameof(BUpload.Width), uploadConfig.Width);
            renderTreeBuilder.AddAttribute(3, nameof(BUpload.MaxSize), uploadConfig.MaxSize);
            renderTreeBuilder.AddAttribute(4, nameof(BUpload.Tip), (RenderFragment)(builder => builder.AddMarkupContent(9, uploadConfig.Tip)));
            renderTreeBuilder.AddAttribute(5, nameof(BUpload.AllowExtensions), uploadConfig.AllowExtensions);
            renderTreeBuilder.AddAttribute(6, nameof(BUpload.Height), uploadConfig.Height);
            renderTreeBuilder.AddAttribute(7, nameof(BUpload.EnablePasteUpload), uploadConfig.EnablePasteUpload);
            renderTreeBuilder.AddAttribute(8, nameof(BUpload.UploadType), uploadConfig.Type);
            renderTreeBuilder.CloseComponent();
        }
    }
}

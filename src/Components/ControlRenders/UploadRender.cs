using Element.ControlConfigs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Element.ControlRenders
{
    internal class UploadRender : IUploadRender
    {
        public void Render(RenderTreeBuilder renderTreeBuilder, RenderConfig config)
        {
            var uploadConfig = (UploadAttribute)config.ControlAttribute;
            renderTreeBuilder.OpenComponent<ElUpload>(0);
            renderTreeBuilder.AddAttribute(1, nameof(ElUpload.Url), uploadConfig.Url);
            renderTreeBuilder.AddAttribute(2, nameof(ElUpload.Width), uploadConfig.Width);
            renderTreeBuilder.AddAttribute(3, nameof(ElUpload.MaxSize), uploadConfig.MaxSize);
            renderTreeBuilder.AddAttribute(4, nameof(ElUpload.Tip), (RenderFragment)(builder => builder.AddMarkupContent(9, uploadConfig.Tip)));
            renderTreeBuilder.AddAttribute(5, nameof(ElUpload.AllowExtensions), uploadConfig.AllowExtensions);
            renderTreeBuilder.AddAttribute(6, nameof(ElUpload.Height), uploadConfig.Height);
            renderTreeBuilder.AddAttribute(7, nameof(ElUpload.EnablePasteUpload), uploadConfig.EnablePasteUpload);
            renderTreeBuilder.AddAttribute(8, nameof(ElUpload.UploadType), uploadConfig.Type);
            renderTreeBuilder.AddAttribute(9, nameof(ElUpload.ListType), uploadConfig.ListType);
            renderTreeBuilder.AddAttribute(10, nameof(ElUpload.Drag), uploadConfig.Drag);
            renderTreeBuilder.AddAttribute(11, nameof(ElUpload.Limit), uploadConfig.Limit);
            renderTreeBuilder.AddAttribute(12, nameof(ElUpload.Multiple), uploadConfig.Multiple);
            renderTreeBuilder.AddAttribute(13, nameof(ElUpload.AutoUpload), uploadConfig.AutoUpload);
            renderTreeBuilder.AddAttribute(14, nameof(ElUpload.ShowFileList), uploadConfig.ShowFileList);
            renderTreeBuilder.AddAttribute(15, nameof(ElUpload.Disabled), uploadConfig.Disabled);
            renderTreeBuilder.AddAttribute(16, nameof(ElUpload.Accept), uploadConfig.Accept);
            renderTreeBuilder.AddAttribute(17, nameof(ElUpload.Method), uploadConfig.Method);
            renderTreeBuilder.AddAttribute(18, nameof(ElUpload.FileFieldName), uploadConfig.FileFieldName);
            renderTreeBuilder.AddAttribute(19, nameof(ElUpload.WithCredentials), uploadConfig.WithCredentials);
            renderTreeBuilder.AddAttribute(20, nameof(ElFormItemObject.EnableAlwaysRender), true);
            renderTreeBuilder.CloseComponent();
        }
    }
}

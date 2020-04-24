using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Markdown.IconHandlers
{
    public class ImageHandler : IIconHandler
    {
        private readonly IJSRuntime jSRuntime;
        private readonly DialogService dialogService;

        public ImageHandler(IJSRuntime jSRuntime, DialogService dialogService)
        {
            this.jSRuntime = jSRuntime;
            this.dialogService = dialogService;
        }

        public async Task HandleAsync(BMarkdownEditorBase editor)
        {
            var imageName = await jSRuntime.InvokeAsync<string>("getSelection", editor.textarea);
            var imageModel = new ImageModel
            {
                Alt = imageName,
                Title = imageName
            };
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(Image.Image), imageModel);
            parameters.Add(nameof(Image.UploadUrl), editor.UploadUrl);
            parameters.Add(nameof(Image.MaxSize), editor.ImageMaxSize);
            parameters.Add(nameof(Image.Width), editor.ImageWidth);
            parameters.Add(nameof(Image.Height), editor.ImageHeight);
            parameters.Add(nameof(Image.AllowExtensions), editor.AllowImageExtensions);
            parameters.Add(nameof(Image.DisableUpload), editor.DisableImageUpload);
            parameters.Add(nameof(Image.Tip), editor.ImageUploadTip);
            var result = await dialogService.ShowDialogAsync<Image, ImageModel>("插入图片", parameters);
            imageModel = result.Result;
            if(imageModel!=null)
            {
                var title = imageModel.Title;
                if (!string.IsNullOrWhiteSpace(title))
                {
                    title = $"\"{title}\"";
                }
                if(imageModel.Urls!=null)
                {
                    var images = imageModel.Urls.Select(url => $"![{imageModel.Alt}]({url} {title})");
                    var image = string.Join("\n", images);
                    await jSRuntime.InvokeVoidAsync("replaceSelection", editor.Textarea, image);
                }
            }
        }
    }
}

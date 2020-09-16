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

        public async Task HandleAsync(BMarkdownEditor editor)
        {
            var imageName = await jSRuntime.InvokeAsync<string>("getSelection", editor.textarea);
            var imageModel = new ImageModel
            {
                Alt = imageName,
                Title = imageName
            };
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(ImageUpload.Image), imageModel);
            parameters.Add(nameof(ImageUpload.UploadUrl), editor.UploadUrl);
            parameters.Add(nameof(ImageUpload.MaxSize), editor.ImageMaxSize);
            parameters.Add(nameof(ImageUpload.Width), editor.ImageWidth);
            parameters.Add(nameof(ImageUpload.Height), editor.ImageHeight);
            parameters.Add(nameof(ImageUpload.AllowExtensions), editor.AllowImageExtensions);
            parameters.Add(nameof(ImageUpload.DisableUpload), editor.DisableImageUpload);
            parameters.Add(nameof(ImageUpload.Tip), editor.ImageUploadTip);
            var result = await dialogService.ShowDialogAsync<ImageUpload, ImageModel>("插入图片", parameters);
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

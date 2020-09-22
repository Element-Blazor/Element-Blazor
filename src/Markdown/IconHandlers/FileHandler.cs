using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.Markdown.IconHandlers
{
    public class FileHandler : IIconHandler
    {
        private readonly IJSRuntime jSRuntime;
        private readonly DialogService dialogService;

        public FileHandler(IJSRuntime jSRuntime, DialogService dialogService)
        {
            this.jSRuntime = jSRuntime;
            this.dialogService = dialogService;
        }

        public async Task HandleAsync(BMarkdownEditor editor)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(FileUpload.UploadUrl), editor.UploadUrl);
            parameters.Add(nameof(FileUpload.MaxSize), editor.FileMaxSize);
            parameters.Add(nameof(FileUpload.AllowExtensions), editor.AllowFileExtensions);
            parameters.Add(nameof(FileUpload.DisableUpload), editor.DisableFileUpload);
            parameters.Add(nameof(FileUpload.Tip), editor.FileUploadTip);
            var result = await dialogService.ShowDialogAsync<FileUpload, FileModel>("插入文件", parameters);
            var fileModel = result.Result;
            if(fileModel!=null)
            {
                var title = fileModel.Title;
                if (!string.IsNullOrWhiteSpace(title))
                {
                    title = $"\"{title}\"";
                }
                if(fileModel.Urls!=null)
                {
                    var files = fileModel.Urls.Select(x => $"[{fileModel?.Name}]({x} {title})");
                    var file = string.Join("\n", files);
                    await jSRuntime.InvokeVoidAsync("replaceSelection", editor.Textarea, file);
                }
            }
        }
    }
}

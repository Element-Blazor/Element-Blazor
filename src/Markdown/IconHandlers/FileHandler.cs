using Blazui.Component;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Markdown.IconHandlers
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

        public async Task HandleAsync(BMarkdownEditorBase editor)
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(File.UploadUrl), editor.UploadUrl);
            parameters.Add(nameof(File.MaxSize), editor.FileMaxSize);
            parameters.Add(nameof(File.AllowExtensions), editor.AllowFileExtensions);
            parameters.Add(nameof(File.DisableUpload), editor.DisableFileUpload);
            parameters.Add(nameof(File.Tip), editor.FileUploadTip);
            var result = await dialogService.ShowDialogAsync<File, FileModel>("插入文件", parameters);
            var fileModel = result.Result;
            var title = fileModel.Title;
            if (!string.IsNullOrWhiteSpace(title))
            {
                title = $"\"{title}\"";
            }
            var files = fileModel.Urls.Select(x => $"[{fileModel.Name}]({x} {title})");
            var file = string.Join("\n", files);
            await jSRuntime.InvokeVoidAsync("replaceSelection", editor.Textarea, file);
        }
    }
}

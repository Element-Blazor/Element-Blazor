using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Upload
{
    public class BUploadBase : BComponentBase
    {
        /// <summary>
        /// 文件上传地址
        /// </summary>
        [Parameter]
        public string Url { get; set; }

        /// <summary>
        /// 上传界面的小提示
        /// </summary>
        [Parameter]
        public RenderFragment Tip { get; set; }

        /// <summary>
        /// 文件删除时触发
        /// </summary>
        [Parameter]
        public EventCallback<UploadModel> OnDeleteFile { get; set; }

        /// <summary>
        /// 允许上传的文件后缀，以“.”开头
        /// </summary>
        [Parameter]
        public string[] AllowExtensions { get; set; } = new string[0];
        internal ElementReference Input { get; set; }

        internal HashSet<UploadModel> Files { get; set; } = new HashSet<UploadModel>();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrWhiteSpace(Url))
            {
                throw new BlazuiException("文件上传地址未指定");
            }
        }

        internal void DeleteFile(UploadModel file)
        {
            Files.Remove(file);
            if (!OnDeleteFile.HasDelegate)
            {
                return;
            }
            _ = OnDeleteFile.InvokeAsync(file);
        }

        internal async Task SelectFileAsync(MouseEventArgs args)
        {
            await Input.Dom(JSRuntime).ClickAsync();
        }

        internal async Task ScanFileAsync()
        {
            var input = Input.Dom(JSRuntime);
            var files = await input.ScanFilesAsync();
            foreach (var item in files)
            {
                var ext = Path.GetExtension(item);
                if (AllowExtensions.Any() && !AllowExtensions.Contains(ext, StringComparer.CurrentCultureIgnoreCase))
                {
                    Alert("您选择的文件中包含不允许上传的文件后缀");
                    return;
                }
                var file = new UploadModel()
                {
                    FileName = Path.GetFileName(item),
                    Status = UploadStatus.UnStart
                };
                Files.Add(file);
            }

            _ = UploadFilesAsync(input);
        }

        private async Task UploadFilesAsync(Element input)
        {
            foreach (var item in Files)
            {
                var results = await input.UploadFileAsync(item.FileName, Url);
                if (results[0] == "0")
                {
                    item.Status = UploadStatus.Success;
                }
                else
                {
                    item.Status = UploadStatus.Failure;
                }
                item.Message = results[1];
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}

using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BUploadBase : BComponentBase
    {
        internal ElementReference hdnField;
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

        /// <summary>
        /// 最大上传文件大小，以 KB 为单位
        /// </summary>
        [Parameter]
        public long MaxSize { get; set; }

        /// <summary>
        /// 对图片文件限制宽度
        /// </summary>
        [Parameter]
        public float Width { get; set; }

        /// <summary>
        /// 对图片文件限制高度
        /// </summary>
        [Parameter]
        public float Height { get; set; }
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
            await input.ClearAsync();
            foreach (var item in files)
            {
                var ext = Path.GetExtension(item[0]);
                if (AllowExtensions.Any() && !AllowExtensions.Contains(ext, StringComparer.CurrentCultureIgnoreCase))
                {
                    Alert("您选择的文件中包含不允许上传的文件后缀");
                    return;
                }
                var size = Convert.ToInt64(item[1]);
                if (size / 1000 > MaxSize)
                {
                    Alert("您选择的文件中包含大小超过允许大小的文件");
                    return;
                }
                if (item.Length == 4)
                {
                    if ((Convert.ToInt32(item[2]) > Width && Width > 0) || (Convert.ToInt32(item[3]) > Height && Height > 0))
                    {
                        Alert("您选择的文件中包含尺寸超过允许大小的文件");
                        return;
                    }
                }
                var file = new UploadModel()
                {
                    FileName = Path.GetFileName(item[0]),
                    Status = UploadStatus.UnStart
                };
                Files.Add(file);
            }

            RequireRender = true;
            _ = UploadFilesAsync(input);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            AllowExtensions = AllowExtensions.Select(x => x?.Trim()).ToArray();
        }

        private async Task UploadFilesAsync(Element input)
        {
            foreach (var item in Files)
            {
                if (item.Status != UploadStatus.UnStart)
                {
                    continue;
                }
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
                RequireRender = true;
                await InvokeAsync(StateHasChanged);
            }
        }
    }
}

using Blazui.Component.Dom;
using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BUploadBase : BFieldComponentBase<IFileModel[]>
    {
        [Inject]
        internal Document Document { get; set; }
        internal ElementReference hdnField;

        private bool eventRegistered = false;
        /// <summary>
        /// 启用粘贴上传
        /// </summary>
        [Parameter]
        public bool EnablePasteUpload { get; set; } = true;

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
        public EventCallback<IFileModel> OnDeleteFile { get; set; }

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
        /// 当文件上传成功时触发
        /// </summary>
        [Parameter]
        public EventCallback<IFileModel> OnFileUploadSuccess { get; set; }
        /// <summary>
        /// 当文件上传失败时触发
        /// </summary>
        [Parameter]
        public EventCallback<IFileModel> OnFileUploadFailure { get; set; }
        /// <summary>
        /// 当文件列表上传完成时触发
        /// </summary>
        [Parameter]
        public EventCallback<IFileModel[]> OnFileListUpload { get; set; }
        /// <summary>
        /// 当文件开始上传时触发
        /// </summary>
        [Parameter]
        public EventCallback<IFileModel> OnFileUploadStart { get; set; }
        /// <summary>
        /// 对图片文件限制高度
        /// </summary>
        [Parameter]
        public float Height { get; set; }

        /// <summary>
        /// 上传类型
        /// </summary>
        [Parameter]
        public UploadType UploadType { get; set; }
        internal ElementReference Input { get; set; }

        internal HashSet<IFileModel> Files { get; set; } = new HashSet<IFileModel>();

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            RequireRender = true;
            if (value == null)
            {
                Files = new HashSet<IFileModel>();
            }
            else
            {
                InitilizeFiles(value as IFileModel[]);
            }
            StateHasChanged();
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (string.IsNullOrWhiteSpace(Url))
            {
                throw new BlazuiException("文件上传地址未指定");
            }
        }

        internal void DeleteFile(IFileModel file)
        {
            Files.Remove(file);
            RequireRender = true;
            SetFieldValue(Files.ToArray(), true);
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

        private void ScanFiles(string[][] files)
        {
            foreach (var item in files)
            {
                var ext = Path.GetExtension(item[0]);
                if (AllowExtensions.Any() && !AllowExtensions.Contains(ext, StringComparer.CurrentCultureIgnoreCase))
                {
                    Alert("您选择的文件中包含不允许上传的文件后缀");
                    return;
                }
                var size = Convert.ToInt64(item[1]);
                if (size / 1000 > MaxSize && MaxSize > 0)
                {
                    Alert("您选择的文件中包含大小超过允许大小的文件");
                    return;
                }
                if (item.Length >= 4)
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
                    Status = UploadStatus.UnStart,
                    Url = item.Length == 5 ? item[4] : string.Empty
                };
                Files.Add(file);
            }
        }

        internal async Task ScanFileAsync()
        {
            var input = Input.Dom(JSRuntime);
            var files = await input.ScanFilesAsync();
            await input.ClearAsync();
            ScanFiles(files);
            _ = UploadFilesAsync(input);
            RequireRender = true;
            SetFieldValue(Files.ToArray(), true);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            AllowExtensions = AllowExtensions.Select(x => x?.Trim()).ToArray();
            if (FormItem == null)
            {
                return;
            }
            if (FormItem.OriginValueHasRendered)
            {
                return;
            }
            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                InitilizeFiles(FormItem.OriginValue);
            }
        }

        private void InitilizeFiles(IFileModel[] fileModels)
        {
            Files = new HashSet<IFileModel>();
            if (fileModels == null)
            {
                return;
            }
            foreach (var item in fileModels)
            {
                Files.Add(new UploadModel()
                {
                    Url = item.Url,
                    FileName = item.FileName,
                    Id = item.Id,
                    Status = UploadStatus.Success
                });
            }
        }

        private async Task UploadFilesAsync(Element input)
        {
            foreach (var item in Files)
            {
                var model = item as UploadModel;
                if (model.Status != UploadStatus.UnStart)
                {
                    continue;
                }
                if (OnFileUploadStart.HasDelegate)
                {
                    _ = OnFileUploadStart.InvokeAsync(item);
                }
                var results = await input.UploadFileAsync(item.FileName, Url);
                await FileUploadedAsync(model, results);
            }
            if (OnFileListUpload.HasDelegate)
            {
                _ = OnFileListUpload.InvokeAsync(Files.ToArray());
            }
        }

        private async Task FileUploadedAsync(UploadModel model, string[] results)
        {
            if (results[0] == "0")
            {
                model.Status = UploadStatus.Success;
                if (OnFileUploadSuccess.HasDelegate)
                {
                    _ = OnFileUploadSuccess.InvokeAsync(model);
                }
            }
            else
            {
                model.Status = UploadStatus.Failure;
                if (OnFileUploadFailure.HasDelegate)
                {
                    _ = OnFileUploadFailure.InvokeAsync(model);
                }
            }
            model.Message = results[1];
            model.Id = results[2];
            model.Url = results[3];
            RequireRender = true;
            await InvokeAsync(StateHasChanged);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (EnablePasteUpload && !eventRegistered)
            {
                eventRegistered = true;
                _ = Document.RegisterPasteUploadAsync(this, Url);
            }
        }

        [JSInvokable("previewFiles")]
        public string[] PasteUploadFiles(string[][] files)
        {
            ScanFiles(files);
            RequireRender = true;
            StateHasChanged();
            return Files.Select(x => x.Id).ToArray();
        }

        [JSInvokable("fileUploaded")]
        public async Task FileUploadedAsync(string[] file, string id)
        {
            await FileUploadedAsync((UploadModel)Files.FirstOrDefault(x => x.Id == id), file);
            RequireRender = true;
            StateHasChanged();
        }
    }
}

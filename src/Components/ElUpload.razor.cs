using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElUpload : ElementFieldComponentBase<IFileModel[]>
    {
        [Inject]
        internal Document Document { get; set; }

        internal ElementReference hdnField;
        internal ElementReference Input { get; set; }
        internal HashSet<IFileModel> Files { get; set; } = new HashSet<IFileModel>();

        private bool eventRegistered;

        [Parameter]
        public bool EnablePasteUpload { get; set; } = true;

        [Parameter]
        public string Url { get; set; }

        [Parameter]
        public string Action
        {
            get => Url;
            set => Url = value;
        }

        [Parameter]
        public string Method { get; set; } = "post";

        [Parameter]
        public RenderFragment Tip { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public RenderFragment<UploadModel> FileTemplate { get; set; }

        [Parameter]
        public EventCallback<IFileModel> OnDeleteFile { get; set; }

        [Parameter]
        public EventCallback<IFileModel> OnPreview { get; set; }

        [Parameter]
        public string[] AllowExtensions { get; set; } = Array.Empty<string>();

        [Parameter]
        public string Accept { get; set; }

        [Parameter]
        public long MaxSize { get; set; }

        [Parameter]
        public float Width { get; set; }

        [Parameter]
        public float Height { get; set; }

        [Parameter]
        public EventCallback<IFileModel> OnFileUploadSuccess { get; set; }

        [Parameter]
        public EventCallback<UploadRequestEventArgs> OnSuccess { get; set; }

        [Parameter]
        public EventCallback<IFileModel> OnFileUploadFailure { get; set; }

        [Parameter]
        public EventCallback<UploadRequestEventArgs> OnError { get; set; }

        [Parameter]
        public EventCallback<IFileModel[]> OnFileListUpload { get; set; }

        [Parameter]
        public EventCallback<IFileModel> OnFileUploadStart { get; set; }

        [Parameter]
        public EventCallback<UploadProgressEventArgs> OnProgress { get; set; }

        [Parameter]
        public EventCallback<UploadChangeEventArgs> OnChange { get; set; }

        [Parameter]
        public EventCallback<UploadExceedEventArgs> OnExceed { get; set; }

        [Parameter]
        public Func<UploadModel, Task<bool>> BeforeUpload { get; set; }

        [Parameter]
        public Func<IFileModel, Task<bool>> BeforeRemove { get; set; }

        [Parameter]
        public Func<UploadRequestContext, Task<UploadRequestResult>> HttpRequest { get; set; }

        [Parameter]
        public UploadType UploadType { get; set; }

        [Parameter]
        public UploadListType ListType { get; set; } = UploadListType.Text;

        [Parameter]
        public bool Drag { get; set; }

        [Parameter]
        public int Limit { get; set; }

        [Parameter]
        public bool Multiple { get; set; } = true;

        [Parameter]
        public bool AutoUpload { get; set; } = true;

        [Parameter]
        public bool ShowFileList { get; set; } = true;

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool Disabled
        {
            get => IsDisabled;
            set => IsDisabled = value;
        }

        [Parameter]
        public string FileFieldName { get; set; } = "fileContent";

        [Parameter]
        public bool WithCredentials { get; set; } = true;

        [Parameter]
        public IDictionary<string, string> Headers { get; set; }

        [Parameter]
        public IDictionary<string, string> Data { get; set; }

        protected UploadListType EffectiveListType =>
            ListType == UploadListType.Text && UploadType == UploadType.Image
                ? UploadListType.Picture
                : ListType;

        protected bool UploadDisabled => IsDisabled || (Limit > 0 && Files.Count >= Limit) || (FormItem?.Form?.Disabled ?? false);

        protected string UploadDisabledText => UploadDisabled ? "true" : "false";

        protected string HiddenValue => string.Join(",", Files.Select(x => x.Id).Where(x => !string.IsNullOrWhiteSpace(x)));

        protected string ListTypeName => EffectiveListType switch
        {
            UploadListType.Picture => "picture",
            UploadListType.PictureCard => "picture-card",
            _ => "text"
        };

        protected string UploadTriggerClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-upload")
            .AddIf(EffectiveListType == UploadListType.PictureCard, "el-upload--picture-card")
            .AddIf(EffectiveListType != UploadListType.PictureCard, Drag ? "el-upload--drag" : "el-upload--text")
            .AddIf(UploadDisabled, "is-disabled")
            .Add(Cls)
            .ToString();

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
            if (string.IsNullOrWhiteSpace(Url) && HttpRequest == null)
            {
                throw new ElementException("Upload action is required.");
            }
        }

        internal async Task DeleteFile(IFileModel file)
        {
            if (BeforeRemove != null && !await BeforeRemove(file))
            {
                return;
            }

            Files.Remove(file);
            RequireRender = true;
            SetFieldValue(Files.ToArray(), true);
            if (OnDeleteFile.HasDelegate)
            {
                await OnDeleteFile.InvokeAsync(file);
            }
            await NotifyChangeAsync(file);
        }

        internal async Task SelectFileAsync(MouseEventArgs args)
        {
            await Input.Dom(JSRuntime).ClickAsync();
        }

        private async Task<List<UploadModel>> ScanFilesAsync(string[][] files)
        {
            var acceptedFiles = new List<UploadModel>();
            if (files == null || files.Length == 0)
            {
                return acceptedFiles;
            }

            var candidates = files.Select(CreateUploadModel).ToList();
            if (Limit > 0 && Files.Count + candidates.Count > Limit)
            {
                await NotifyExceedAsync(candidates);
                return acceptedFiles;
            }

            foreach (var file in candidates)
            {
                if (!ValidateFile(file))
                {
                    return acceptedFiles;
                }
                if (BeforeUpload != null && !await BeforeUpload(file))
                {
                    continue;
                }

                Files.Add(file);
                acceptedFiles.Add(file);
                await NotifyChangeAsync(file);
            }

            return acceptedFiles;
        }

        private UploadModel CreateUploadModel(string[] item)
        {
            var model = new UploadModel
            {
                FileName = Path.GetFileName(item[0]),
                Status = UploadStatus.UnStart,
                Url = item.Length == 5 ? item[4] : string.Empty
            };

            if (item.Length > 1 && long.TryParse(item[1], NumberStyles.Integer, CultureInfo.InvariantCulture, out var size))
            {
                model.Size = size;
            }
            if (item.Length > 2 && int.TryParse(item[2], NumberStyles.Integer, CultureInfo.InvariantCulture, out var width))
            {
                model.Width = width;
            }
            if (item.Length > 3 && int.TryParse(item[3], NumberStyles.Integer, CultureInfo.InvariantCulture, out var height))
            {
                model.Height = height;
            }

            return model;
        }

        private bool ValidateFile(UploadModel file)
        {
            var ext = Path.GetExtension(file.FileName);
            if (AllowExtensions.Any() && !AllowExtensions.Contains(ext, StringComparer.CurrentCultureIgnoreCase))
            {
                Alert("Selected files include a file extension that is not allowed.");
                return false;
            }

            if (file.Size / 1000 > MaxSize && MaxSize > 0)
            {
                Alert("Selected files include a file larger than the allowed size.");
                return false;
            }

            if ((file.Width > Width && Width > 0) || (file.Height > Height && Height > 0))
            {
                Alert("Selected files include an image larger than the allowed dimensions.");
                return false;
            }

            return true;
        }

        internal async Task ScanFileAsync()
        {
            var input = Input.Dom(JSRuntime);
            var files = await input.ScanFilesAsync();
            var acceptedFiles = await ScanFilesAsync(files);
            if (AutoUpload)
            {
                await UploadFilesAsync(input, acceptedFiles);
            }
            else
            {
                RequireRender = true;
                SetFieldValue(Files.ToArray(), true);
                StateHasChanged();
            }
        }

        public async Task SubmitAsync()
        {
            await UploadFilesAsync(Input.Dom(JSRuntime), Files.Cast<UploadModel>().Where(x => x.Status == UploadStatus.UnStart).ToList());
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            AllowExtensions = AllowExtensions?.Select(x => x?.Trim()).Where(x => !string.IsNullOrWhiteSpace(x)).ToArray() ?? Array.Empty<string>();
            if (string.IsNullOrWhiteSpace(Accept) && AllowExtensions.Any())
            {
                Accept = string.Join(",", AllowExtensions);
            }
            if (FormItem == null || FormItem.OriginValueHasRendered)
            {
                return;
            }

            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                InitilizeFiles(FormItem.OriginValue as IFileModel[]);
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
                Files.Add(new UploadModel
                {
                    Url = item.Url,
                    FileName = item.FileName,
                    Id = item.Id,
                    Status = UploadStatus.Success
                });
            }
        }

        private async Task UploadFilesAsync(ElementHelper input, IEnumerable<UploadModel> files)
        {
            foreach (var model in files.Where(x => x.Status == UploadStatus.UnStart).ToList())
            {
                model.Status = UploadStatus.Uploading;
                await NotifyProgressAsync(model, 0);
                if (OnFileUploadStart.HasDelegate)
                {
                    await OnFileUploadStart.InvokeAsync(model);
                }

                UploadRequestResult result;
                if (HttpRequest != null)
                {
                    result = await HttpRequest(CreateRequestContext(model, input));
                }
                else
                {
                    result = UploadRequestResult.FromJsResult(await input.UploadFileAsync(
                        model.FileName,
                        Url,
                        Method,
                        FileFieldName,
                        WithCredentials,
                        Headers,
                        Data));
                }

                await FileUploadedAsync(model, result);
            }

            await input.ClearAsync();
            RequireRender = true;
            if (OnFileListUpload.HasDelegate)
            {
                await OnFileListUpload.InvokeAsync(Files.ToArray());
            }
            StateHasChanged();
        }

        private UploadRequestContext CreateRequestContext(UploadModel file, ElementHelper input)
        {
            return new UploadRequestContext
            {
                File = file,
                FileList = Files.ToArray(),
                Url = Url,
                Method = Method,
                FileFieldName = FileFieldName,
                WithCredentials = WithCredentials,
                Headers = Headers,
                Data = Data,
                Input = input,
                ReportProgress = value => NotifyProgressAsync(file, value)
            };
        }

        private async Task FileUploadedAsync(UploadModel model, UploadRequestResult result)
        {
            if (model == null)
            {
                return;
            }

            result ??= UploadRequestResult.Failure("Upload failed.");
            model.Message = result.Message;
            model.Id = string.IsNullOrWhiteSpace(result.Id) ? model.Id : result.Id;
            model.Url = string.IsNullOrWhiteSpace(result.Url) ? model.Url : result.Url;
            model.Status = result.Succeeded ? UploadStatus.Success : UploadStatus.Failure;

            await NotifyProgressAsync(model, result.Succeeded ? 100 : 0);
            var requestArgs = new UploadRequestEventArgs
            {
                File = model,
                FileList = Files.ToArray(),
                Response = result
            };

            if (result.Succeeded)
            {
                if (OnFileUploadSuccess.HasDelegate)
                {
                    await OnFileUploadSuccess.InvokeAsync(model);
                }
                if (OnSuccess.HasDelegate)
                {
                    await OnSuccess.InvokeAsync(requestArgs);
                }
            }
            else
            {
                if (OnFileUploadFailure.HasDelegate)
                {
                    await OnFileUploadFailure.InvokeAsync(model);
                }
                if (OnError.HasDelegate)
                {
                    await OnError.InvokeAsync(requestArgs);
                }
            }

            await NotifyChangeAsync(model);
        }

        private async Task NotifyProgressAsync(IFileModel file, double percent)
        {
            if (!OnProgress.HasDelegate)
            {
                return;
            }

            await OnProgress.InvokeAsync(new UploadProgressEventArgs
            {
                File = file,
                FileList = Files.ToArray(),
                Percent = percent
            });
        }

        private async Task NotifyChangeAsync(IFileModel file)
        {
            SetFieldValue(Files.ToArray(), true);
            if (!OnChange.HasDelegate)
            {
                return;
            }

            await OnChange.InvokeAsync(new UploadChangeEventArgs
            {
                File = file,
                FileList = Files.ToArray()
            });
        }

        private async Task NotifyExceedAsync(IEnumerable<UploadModel> files)
        {
            if (OnExceed.HasDelegate)
            {
                await OnExceed.InvokeAsync(new UploadExceedEventArgs
                {
                    Files = files.ToArray(),
                    FileList = Files.ToArray()
                });
                return;
            }

            Alert("The selected files exceed the upload limit.");
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (EnablePasteUpload && !eventRegistered && !string.IsNullOrWhiteSpace(Url))
            {
                eventRegistered = true;
                _ = Document.RegisterPasteUploadAsync(this, Url, CreateUploadOptions());
            }
        }

        [JSInvokable("previewFiles")]
        public async Task<string[]> PasteUploadFiles(string[][] files)
        {
            var acceptedFiles = await ScanFilesAsync(files);
            RequireRender = true;
            StateHasChanged();
            return acceptedFiles.Select(x => x.Id).ToArray();
        }

        [JSInvokable("fileUploaded")]
        public async Task FileUploadedFromJsAsync(string[] file, string id)
        {
            await FileUploadedAsync(Files.OfType<UploadModel>().FirstOrDefault(x => x.Id == id), UploadRequestResult.FromJsResult(file));
            RequireRender = true;
            StateHasChanged();
        }

        [JSInvokable("filesUploaded")]
        public async Task FilesUploaded()
        {
            SetFieldValue(Files.ToArray(), true);
            RequireRender = true;
            if (OnFileListUpload.HasDelegate)
            {
                await OnFileListUpload.InvokeAsync(Files.ToArray());
            }
            else
            {
                StateHasChanged();
            }
        }

        protected string GetFileItemClass(UploadModel file)
        {
            return HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-upload-list__item")
                .AddIf(file.Status == UploadStatus.Success, "is-success")
                .AddIf(file.Status == UploadStatus.Uploading, "is-uploading")
                .AddIf(file.Status == UploadStatus.Failure, "is-fail")
                .ToString();
        }

        protected void BuildTextOrPictureItem(RenderTreeBuilder builder, UploadModel file, ref int seq)
        {
            if (EffectiveListType == UploadListType.Picture)
            {
                builder.OpenElement(seq++, "img");
                builder.AddAttribute(seq++, "src", file.Url);
                builder.AddAttribute(seq++, "alt", string.Empty);
                builder.AddAttribute(seq++, "class", "el-upload-list__item-thumbnail");
                builder.CloseElement();
            }

            builder.OpenElement(seq++, "a");
            builder.AddAttribute(seq++, "class", "el-upload-list__item-name");
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => PreviewFileAsync(file)));
            builder.OpenElement(seq++, "i");
            builder.AddAttribute(seq++, "class", "el-icon-document");
            builder.CloseElement();
            builder.AddContent(seq++, file.FileName);
            builder.CloseElement();

            BuildStatusLabel(builder, file, ref seq);
            BuildDeleteIcon(builder, file, ref seq, "el-icon-close");

            builder.OpenElement(seq++, "i");
            builder.AddAttribute(seq++, "class", "el-icon-close-tip");
            builder.CloseElement();
        }

        protected void BuildPictureCardItem(RenderTreeBuilder builder, UploadModel file, ref int seq)
        {
            builder.OpenElement(seq++, "img");
            builder.AddAttribute(seq++, "src", file.Url);
            builder.AddAttribute(seq++, "alt", file.FileName);
            builder.AddAttribute(seq++, "class", "el-upload-list__item-thumbnail");
            builder.CloseElement();

            BuildStatusLabel(builder, file, ref seq);

            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", "el-upload-list__item-actions");
            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", "el-upload-list__item-preview");
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => PreviewFileAsync(file)));
            builder.OpenElement(seq++, "i");
            builder.AddAttribute(seq++, "class", "el-icon-zoom-in");
            builder.CloseElement();
            builder.CloseElement();

            builder.OpenElement(seq++, "span");
            builder.AddAttribute(seq++, "class", "el-upload-list__item-delete");
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => DeleteFile(file)));
            builder.OpenElement(seq++, "i");
            builder.AddAttribute(seq++, "class", "el-icon-delete");
            builder.CloseElement();
            builder.CloseElement();
            builder.CloseElement();
        }

        private void BuildStatusLabel(RenderTreeBuilder builder, UploadModel file, ref int seq)
        {
            if (file.Status != UploadStatus.Success)
            {
                return;
            }

            builder.OpenElement(seq++, "label");
            builder.AddAttribute(seq++, "class", "el-upload-list__item-status-label");
            builder.OpenElement(seq++, "i");
            builder.AddAttribute(seq++, "class", "el-icon-upload-success el-icon-circle-check");
            builder.CloseElement();
            builder.CloseElement();
        }

        private void BuildDeleteIcon(RenderTreeBuilder builder, UploadModel file, ref int seq, string cls)
        {
            builder.OpenElement(seq++, "i");
            builder.AddAttribute(seq++, "class", cls);
            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, _ => DeleteFile(file)));
            builder.CloseElement();
        }

        private object CreateUploadOptions()
        {
            return new
            {
                method = Method,
                fileFieldName = FileFieldName,
                withCredentials = WithCredentials,
                headers = Headers,
                data = Data
            };
        }

        private async Task PreviewFileAsync(IFileModel file)
        {
            if (OnPreview.HasDelegate)
            {
                await OnPreview.InvokeAsync(file);
            }
        }

        public override void Dispose()
        {
            base.Dispose();
            if (eventRegistered)
            {
                _ = Document.UnRegisterPasteUploadAsync();
            }
        }
    }
}

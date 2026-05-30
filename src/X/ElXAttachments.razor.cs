using Element;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element.X
{
    public partial class ElXAttachments : ElementComponentBase
    {
        [Parameter]
        public IList<XAttachmentItem> Items { get; set; } = new List<XAttachmentItem>();

        [Parameter]
        public EventCallback<IList<XAttachmentItem>> ItemsChanged { get; set; }

        [Parameter]
        public EventCallback<XAttachmentItem> OnRemove { get; set; }

        [Parameter]
        public EventCallback<XAttachmentItem> OnPreview { get; set; }

        [Parameter]
        public EventCallback<UploadProgressEventArgs> OnProgress { get; set; }

        [Parameter]
        public EventCallback<UploadRequestEventArgs> OnSuccess { get; set; }

        [Parameter]
        public EventCallback<UploadRequestEventArgs> OnError { get; set; }

        [Parameter]
        public EventCallback<UploadExceedEventArgs> OnExceed { get; set; }

        [Parameter]
        public EventCallback<UploadChangeEventArgs> OnChange { get; set; }

        [Parameter]
        public EventCallback<XAttachmentUploadEventArgs> OnStatusChanged { get; set; }

        [Parameter]
        public EventCallback<XAttachmentUploadEventArgs> OnUploadException { get; set; }

        [Parameter]
        public EventCallback<XAttachmentUploadExceedEventArgs> OnUploadExceed { get; set; }

        [Parameter]
        public RenderFragment UploadContent { get; set; }

        [Parameter]
        public RenderFragment<ElXAttachments> UploadTemplate { get; set; }

        [Parameter]
        public RenderFragment TipTemplate { get; set; }

        [Parameter]
        public RenderFragment<XAttachmentItem> ItemTemplate { get; set; }

        [Parameter]
        public RenderFragment EmptyContent { get; set; }

        [Parameter]
        public string UploadUrl { get; set; } = "/";

        [Parameter]
        public string Method { get; set; } = "post";

        [Parameter]
        public bool AutoUpload { get; set; }

        [Parameter]
        public bool Multiple { get; set; } = true;

        [Parameter]
        public int Limit { get; set; }

        [Parameter]
        public string Accept { get; set; }

        [Parameter]
        public string[] AllowExtensions { get; set; } = Array.Empty<string>();

        [Parameter]
        public long MaxSize { get; set; }

        [Parameter]
        public bool Drag { get; set; }

        [Parameter]
        public bool Readonly { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public string UploadText { get; set; } = "Attach";

        [Parameter]
        public string UploadIcon { get; set; } = "el-icon-paperclip";

        [Parameter]
        public string ErrorText { get; set; }

        [Parameter]
        public string ExceedMessage { get; set; } = "The selected files exceed the upload limit.";

        [Parameter]
        public Func<IFileModel, UploadRequestResult, string> ErrorMessageSelector { get; set; }

        [Parameter]
        public Func<IFileModel, XAttachmentStatus> StatusMapper { get; set; }

        [Parameter]
        public Func<UploadModel, Task<bool>> BeforeUpload { get; set; }

        [Parameter]
        public Func<IFileModel, Task<bool>> BeforeRemove { get; set; }

        [Parameter]
        public Func<UploadRequestContext, Task<UploadRequestResult>> HttpRequest { get; set; }

        protected string AttachmentsClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-attachments", Cls)
            .AddIf(Readonly, "is-readonly")
            .AddIf(Disabled, "is-disabled")
            .ToString();

        protected Func<UploadRequestContext, Task<UploadRequestResult>> EffectiveHttpRequest =>
            HttpRequest == null ? null : UploadRequestAsync;

        private async Task HandleUploadChangeAsync(UploadChangeEventArgs args)
        {
            Items = MapFiles(args?.FileList);

            if (OnChange.HasDelegate)
            {
                await OnChange.InvokeAsync(args);
            }

            await NotifyItemsChangedAsync();
            await NotifyStatusChangedAsync(FindItem(args?.File), args?.File);
        }

        private async Task HandleUploadProgressAsync(UploadProgressEventArgs args)
        {
            UpdateItem(args?.File, item =>
            {
                item.Status = XAttachmentStatus.Uploading;
                item.Progress = args.Percent;
                item.Error = null;
                item.ErrorCode = null;
                item.Response = null;
                item.Exception = null;
            });

            if (OnProgress.HasDelegate)
            {
                await OnProgress.InvokeAsync(args);
            }

            await NotifyItemsChangedAsync();
            await NotifyStatusChangedAsync(FindItem(args?.File), args?.File);
        }

        private async Task HandleUploadSuccessAsync(UploadRequestEventArgs args)
        {
            UpdateItem(args?.File, item =>
            {
                item.Status = XAttachmentStatus.Success;
                item.Progress = 100;
                item.Error = null;
                item.ErrorCode = null;
                item.Response = args?.Response;
                item.Exception = null;
            });

            if (OnSuccess.HasDelegate)
            {
                await OnSuccess.InvokeAsync(args);
            }

            await NotifyItemsChangedAsync();
            await NotifyStatusChangedAsync(FindItem(args?.File), args?.File, args?.Response);
        }

        private async Task HandleUploadErrorAsync(UploadRequestEventArgs args)
        {
            UpdateItem(args?.File, item =>
            {
                item.Status = XAttachmentStatus.Error;
                item.Progress = 0;
                item.Error = ResolveErrorMessage(args?.File, args?.Response);
                item.ErrorCode = args?.Response?.Code;
                item.Response = args?.Response;
            });

            if (OnError.HasDelegate)
            {
                await OnError.InvokeAsync(args);
            }

            await NotifyItemsChangedAsync();
            await NotifyStatusChangedAsync(FindItem(args?.File), args?.File, args?.Response);
        }

        private async Task HandleUploadExceedAsync(UploadExceedEventArgs args)
        {
            ErrorText = ExceedMessage;
            if (OnExceed.HasDelegate)
            {
                await OnExceed.InvokeAsync(args);
            }
            if (OnUploadExceed.HasDelegate)
            {
                await OnUploadExceed.InvokeAsync(new XAttachmentUploadExceedEventArgs
                {
                    UploadArgs = args,
                    Message = ExceedMessage
                });
            }
        }

        private IList<XAttachmentItem> MapFiles(IEnumerable<IFileModel> files)
        {
            return files?.Select(MapFile).ToList() ?? new List<XAttachmentItem>();
        }

        private XAttachmentItem MapFile(IFileModel file)
        {
            return new XAttachmentItem
            {
                Id = file?.Id,
                FileName = file?.FileName,
                Url = file?.Url,
                Size = FormatSize(TryGetUploadSize(file)),
                Type = ResolveFileType(file?.FileName),
                Status = MapStatus(file),
                Progress = TryGetUploadStatus(file) == UploadStatus.Success ? 100 : 0,
                Error = TryGetUploadStatus(file) == UploadStatus.Failure
                    ? ResolveErrorMessage(file, null)
                    : null,
                File = file
            };
        }

        private void UpdateItem(IFileModel file, Action<XAttachmentItem> update)
        {
            if (file == null || update == null)
            {
                return;
            }

            Items ??= new List<XAttachmentItem>();
            var item = Items.FirstOrDefault(x => string.Equals(x.Id, file.Id, StringComparison.Ordinal))
                ?? Items.FirstOrDefault(x => !string.IsNullOrWhiteSpace(file.FileName)
                    && string.Equals(x.FileName, file.FileName, StringComparison.Ordinal));
            if (item == null)
            {
                item = MapFile(file);
                Items.Add(item);
            }
            else
            {
                item.Id = file.Id;
                item.FileName = file.FileName;
                item.Url = file.Url;
                item.Size = FormatSize(TryGetUploadSize(file));
                item.Type = ResolveFileType(file.FileName);
                item.File = file;
            }

            update(item);
        }

        private async Task NotifyItemsChangedAsync()
        {
            if (ItemsChanged.HasDelegate)
            {
                await ItemsChanged.InvokeAsync(Items);
            }
        }

        private XAttachmentItem FindItem(IFileModel file)
        {
            if (file == null)
            {
                return null;
            }

            return Items?.FirstOrDefault(x => string.Equals(x.Id, file.Id, StringComparison.Ordinal))
                ?? Items?.FirstOrDefault(x => !string.IsNullOrWhiteSpace(file.FileName)
                    && string.Equals(x.FileName, file.FileName, StringComparison.Ordinal));
        }

        private XAttachmentStatus MapStatus(IFileModel file)
        {
            if (StatusMapper != null)
            {
                return StatusMapper(file);
            }

            var status = TryGetUploadStatus(file);
            return status switch
            {
                UploadStatus.Uploading => XAttachmentStatus.Uploading,
                UploadStatus.Success => XAttachmentStatus.Success,
                UploadStatus.Failure => XAttachmentStatus.Error,
                _ => XAttachmentStatus.Ready
            };
        }

        private static UploadStatus? TryGetUploadStatus(IFileModel file)
        {
            return file is UploadModel model ? model.Status : null;
        }

        private static long TryGetUploadSize(IFileModel file)
        {
            return file is UploadModel model ? model.Size : 0;
        }

        private static string TryGetUploadMessage(IFileModel file)
        {
            return file is UploadModel model ? model.Message : null;
        }

        private string ResolveErrorMessage(IFileModel file, UploadRequestResult response)
        {
            if (ErrorMessageSelector != null)
            {
                return ErrorMessageSelector(file, response);
            }

            return response?.Message ?? TryGetUploadMessage(file) ?? "Upload failed.";
        }

        private static string ResolveFileType(string fileName)
        {
            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            var extension = System.IO.Path.GetExtension(fileName)?.TrimStart('.').ToLowerInvariant();
            if (string.IsNullOrWhiteSpace(extension))
            {
                return null;
            }

            return extension switch
            {
                "png" or "jpg" or "jpeg" or "gif" or "webp" or "bmp" or "svg" => "image",
                "pdf" => "pdf",
                "cs" or "js" or "ts" or "css" or "html" or "json" or "md" or "xml" or "razor" => "code",
                _ => extension
            };
        }

        private static string FormatSize(long size)
        {
            if (size <= 0)
            {
                return null;
            }

            string[] units = { "B", "KB", "MB", "GB" };
            var value = (double)size;
            var unit = 0;
            while (value >= 1024 && unit < units.Length - 1)
            {
                value /= 1024;
                unit++;
            }

            return $"{value.ToString(value >= 10 || unit == 0 ? "0" : "0.#", CultureInfo.InvariantCulture)} {units[unit]}";
        }

        private async Task RemoveAsync(XAttachmentItem item)
        {
            if (item == null)
            {
                return;
            }

            Items?.Remove(item);
            await NotifyItemsChangedAsync();
            if (OnRemove.HasDelegate)
            {
                await OnRemove.InvokeAsync(item);
            }
        }

        private async Task PreviewAsync(XAttachmentItem item)
        {
            if (OnPreview.HasDelegate)
            {
                await OnPreview.InvokeAsync(item);
            }
        }

        private async Task<UploadRequestResult> UploadRequestAsync(UploadRequestContext context)
        {
            try
            {
                var result = HttpRequest != null
                    ? await HttpRequest(context)
                    : UploadRequestResult.Failure("Upload request is not configured.");
                return result ?? UploadRequestResult.Failure("Upload failed.");
            }
            catch (Exception ex)
            {
                var result = UploadRequestResult.Failure(ex.Message);
                UpdateItem(context?.File, item =>
                {
                    item.Status = XAttachmentStatus.Error;
                    item.Progress = 0;
                    item.Error = ResolveErrorMessage(context?.File, result);
                    item.ErrorCode = result.Code;
                    item.Response = result;
                    item.Exception = ex;
                });

                var mappedItem = FindItem(context?.File);
                await NotifyItemsChangedAsync();
                var eventArgs = new XAttachmentUploadEventArgs
                {
                    Item = mappedItem,
                    File = context?.File,
                    Response = result,
                    Exception = ex
                };

                if (OnUploadException.HasDelegate)
                {
                    await OnUploadException.InvokeAsync(eventArgs);
                }
                if (OnStatusChanged.HasDelegate)
                {
                    await OnStatusChanged.InvokeAsync(eventArgs);
                }

                return result;
            }
        }

        private async Task NotifyStatusChangedAsync(XAttachmentItem item, IFileModel file, UploadRequestResult response = null)
        {
            if (!OnStatusChanged.HasDelegate || item == null)
            {
                return;
            }

            await OnStatusChanged.InvokeAsync(new XAttachmentUploadEventArgs
            {
                Item = item,
                File = file,
                Response = response
            });
        }
    }
}

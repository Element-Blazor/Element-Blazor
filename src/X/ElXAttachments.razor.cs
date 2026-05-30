using Element;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
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
        public RenderFragment UploadContent { get; set; }

        [Parameter]
        public string UploadUrl { get; set; } = "/";

        [Parameter]
        public bool AutoUpload { get; set; }

        [Parameter]
        public bool Multiple { get; set; } = true;

        [Parameter]
        public int Limit { get; set; }

        [Parameter]
        public string Accept { get; set; }

        [Parameter]
        public bool Drag { get; set; }

        [Parameter]
        public bool Readonly { get; set; }

        protected string AttachmentsClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-attachments", Cls)
            .AddIf(Readonly, "is-readonly")
            .ToString();

        private async Task HandleUploadChangeAsync(UploadChangeEventArgs args)
        {
            Items = args.FileList?.Select(file => new XAttachmentItem
            {
                Id = file.Id,
                FileName = file.FileName,
                Url = file.Url,
                Status = XAttachmentStatus.Ready
            }).ToList() ?? new List<XAttachmentItem>();

            if (ItemsChanged.HasDelegate)
            {
                await ItemsChanged.InvokeAsync(Items);
            }
        }

        private async Task RemoveAsync(XAttachmentItem item)
        {
            if (item == null)
            {
                return;
            }

            Items?.Remove(item);
            if (ItemsChanged.HasDelegate)
            {
                await ItemsChanged.InvokeAsync(Items);
            }
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
    }
}

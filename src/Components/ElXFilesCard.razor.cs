using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElXFilesCard : ElementComponentBase
    {
        [Parameter]
        public XAttachmentItem File { get; set; }

        [Parameter]
        public bool Compact { get; set; }

        [Parameter]
        public bool Readonly { get; set; }

        [Parameter]
        public EventCallback<XAttachmentItem> OnPreview { get; set; }

        [Parameter]
        public EventCallback<XAttachmentItem> OnRemove { get; set; }

        protected string CardClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-files-card", Cls)
            .AddIf(Compact, "is-compact")
            .AddIf(File?.Status == XAttachmentStatus.Error, "is-error")
            .AddIf(File?.Status == XAttachmentStatus.Uploading, "is-uploading")
            .ToString();

        protected string IconName => File?.Type?.ToLowerInvariant() switch
        {
            "image" => "picture",
            "pdf" => "document",
            "code" => "tickets",
            _ => "document"
        };

        protected string StatusText => File?.Status switch
        {
            XAttachmentStatus.Uploading => "Uploading",
            XAttachmentStatus.Success => "Uploaded",
            XAttachmentStatus.Error => "Failed",
            _ => "Ready"
        };

        private async Task PreviewAsync(MouseEventArgs args)
        {
            if (OnPreview.HasDelegate)
            {
                await OnPreview.InvokeAsync(File);
            }
        }

        private async Task RemoveAsync(MouseEventArgs args)
        {
            if (OnRemove.HasDelegate)
            {
                await OnRemove.InvokeAsync(File);
            }
        }
    }
}

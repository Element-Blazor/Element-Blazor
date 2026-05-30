using Element;
using System;

namespace Element.X
{
    public class XAttachmentUploadEventArgs : EventArgs
    {
        public XAttachmentItem Item { get; set; }

        public IFileModel File { get; set; }

        public UploadRequestResult Response { get; set; }

        public Exception Exception { get; set; }
    }
}

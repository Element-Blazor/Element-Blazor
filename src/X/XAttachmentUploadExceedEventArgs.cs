using Element;
using System;

namespace Element.X
{
    public class XAttachmentUploadExceedEventArgs : EventArgs
    {
        public UploadExceedEventArgs UploadArgs { get; set; }

        public string Message { get; set; }
    }
}

using System;

namespace Element
{
    public class UploadChangeEventArgs : EventArgs
    {
        public IFileModel File { get; set; }

        public IFileModel[] FileList { get; set; } = Array.Empty<IFileModel>();
    }
}

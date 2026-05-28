using System;

namespace Element
{
    public class UploadExceedEventArgs : EventArgs
    {
        public UploadModel[] Files { get; set; } = Array.Empty<UploadModel>();

        public IFileModel[] FileList { get; set; } = Array.Empty<IFileModel>();
    }
}

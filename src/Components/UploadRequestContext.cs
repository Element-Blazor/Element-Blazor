using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element
{
    public class UploadRequestContext
    {
        public UploadModel File { get; set; }

        public IFileModel[] FileList { get; set; } = Array.Empty<IFileModel>();

        public string Url { get; set; }

        public string Method { get; set; }

        public string FileFieldName { get; set; }

        public bool WithCredentials { get; set; }

        public IDictionary<string, string> Headers { get; set; }

        public IDictionary<string, string> Data { get; set; }

        public ElementHelper Input { get; set; }

        public Func<double, Task> ReportProgress { get; set; }
    }
}

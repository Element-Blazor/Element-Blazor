namespace Element
{
    public class XAttachmentItem
    {
        public string Id { get; set; }

        public string FileName { get; set; }

        public string Size { get; set; }

        public string Type { get; set; }

        public string Url { get; set; }

        public double Progress { get; set; }

        public XAttachmentStatus Status { get; set; } = XAttachmentStatus.Ready;

        public string Error { get; set; }
    }
}

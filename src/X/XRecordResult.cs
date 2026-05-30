namespace Element.X
{
    public class XRecordResult
    {
        public bool Supported { get; set; }

        public bool Started { get; set; }

        public string SessionId { get; set; }

        public string State { get; set; }

        public string Message { get; set; }

        public string MimeType { get; set; }

        public string FileName { get; set; }

        public long Size { get; set; }

        public string Base64 { get; set; }
    }
}

namespace Element.X
{
    public class XStreamChunk
    {
        public string Id { get; set; }

        public string Content { get; set; }

        public bool IsStart { get; set; }

        public bool IsEnd { get; set; }

        public bool IsError { get; set; }

        public string Error { get; set; }

        public object Raw { get; set; }
    }
}

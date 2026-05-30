using System.Collections.Generic;

namespace Element.X
{
    public class XRequestResult
    {
        public string RequestId { get; set; }

        public string ConversationId { get; set; }

        public XMessageItem UserMessage { get; set; }

        public XMessageItem AssistantMessage { get; set; }

        public IReadOnlyList<XStreamChunk> Chunks { get; set; } = new List<XStreamChunk>();

        public bool Canceled { get; set; }

        public string Error { get; set; }
    }
}

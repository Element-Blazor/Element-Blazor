using System;
using System.Collections.Generic;

namespace Element.X
{
    public class XStreamRequest
    {
        public string RequestId { get; set; }

        public string ConversationId { get; set; }

        public string Prompt { get; set; }

        public string ResponseText { get; set; }

        public IEnumerable<XMessageItem> Messages { get; set; }

        public IEnumerable<XAttachmentItem> Attachments { get; set; }

        public int ChunkSize { get; set; } = 6;

        public TimeSpan ChunkDelay { get; set; } = TimeSpan.FromMilliseconds(24);
    }
}

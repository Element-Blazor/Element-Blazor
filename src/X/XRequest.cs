using System;
using System.Collections.Generic;
using System.Threading;

namespace Element.X
{
    public class XRequest
    {
        public string RequestId { get; set; }

        public string ConversationId { get; set; }

        public string Prompt { get; set; }

        public string UserName { get; set; } = "You";

        public string AssistantName { get; set; } = "Element X";

        public IEnumerable<XMessageItem> Messages { get; set; }

        public IEnumerable<XAttachmentItem> Attachments { get; set; }

        public Func<XRequest, CancellationToken, IAsyncEnumerable<XStreamChunk>> Transport { get; set; }

        public bool IncludeUserMessage { get; set; } = true;
    }
}

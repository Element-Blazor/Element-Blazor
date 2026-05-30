using System;

namespace Element.X
{
    public class XConversationDeleteEventArgs : EventArgs
    {
        public XConversationItem Item { get; set; }

        public string DeletedId { get; set; }

        public string NextActiveId { get; set; }
    }
}

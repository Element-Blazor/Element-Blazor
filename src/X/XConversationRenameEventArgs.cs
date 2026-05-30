using System;

namespace Element.X
{
    public class XConversationRenameEventArgs : EventArgs
    {
        public XConversationItem Item { get; set; }

        public string OldTitle { get; set; }

        public string Title { get; set; }
    }
}

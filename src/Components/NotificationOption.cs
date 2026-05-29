using Microsoft.AspNetCore.Components;
using System;
using System.Threading.Tasks;

namespace Element
{
    public class NotificationOption
    {
        public string Title { get; set; }

        public string Message { get; set; }

        public RenderFragment MessageContent { get; set; }

        public MessageType Type { get; set; } = MessageType.Info;

        public string Icon { get; set; }

        public string CustomClass { get; set; }

        public NotificationPosition Position { get; set; } = NotificationPosition.TopRight;

        public int Duration { get; set; } = 4500;

        public int Offset { get; set; } = 16;

        public bool ShowClose { get; set; } = true;

        public Func<Task> OnClose { get; set; }

        internal bool IsNew { get; set; }

        internal bool Closing { get; set; }

        internal int ZIndex { get; set; }

        internal int Top { get; set; }

        internal int Bottom { get; set; }
    }
}

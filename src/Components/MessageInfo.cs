using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class MessageInfo
    {
        public override string ToString()
        {
            return Message;
        }
        public string Message { get; set; }
        public int Duration { get; set; }
        public MessageType Type { get; set; } = MessageType.Info;
        internal ElementReference Element { get; set; }
        internal bool IsNew { get; set; }
        internal int Index { get; set; }
        internal int BeginTop { get; set; }
        internal int EndTop { get; set; }
        internal int ZIndex { get; set; }
    }
}


using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Element.Demo.Message
{
    public partial class BasicMessage : BComponentBase
    {
        [Inject]
        public MessageService Message { get; set; }
        
        public void ShowMessage()
        {
            Message.Show("普通消息");
        }

    }
}

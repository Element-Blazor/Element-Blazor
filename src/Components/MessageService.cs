using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class MessageService
    {
        internal ObservableCollection<MessageInfo> Messages = new ObservableCollection<MessageInfo>();

        public void Show(string text)
        {
            Messages.Add(new MessageInfo()
            {
                Message = text,
                Duration = 3000
            });
        }
        public void Show(string text, MessageType type)
        {
            Messages.Add(new MessageInfo()
            {
                Type = type,
                Message = text,
                Duration = 3000
            });
        }
    }
}

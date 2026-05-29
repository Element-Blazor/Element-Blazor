using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class MessageService
    {
        internal ObservableCollection<MessageInfo> Messages = new ObservableCollection<MessageInfo>();

        public MessageInfo Show(string text)
        {
            return Show(new MessageInfo
            {
                Message = text,
                Duration = 3000
            });
        }

        public MessageInfo Show(string text, MessageType type)
        {
            return Show(new MessageInfo
            {
                Type = type,
                Message = text,
                Duration = 3000
            });
        }

        public MessageInfo Show(string text, MessageType type, int duration)
        {
            return Show(new MessageInfo
            {
                Type = type,
                Message = text,
                Duration = duration
            });
        }

        public MessageInfo Show(MessageInfo info)
        {
            if (info == null)
            {
                throw new ArgumentNullException(nameof(info));
            }

            if (info.Grouping)
            {
                var existing = Messages.FirstOrDefault(x => x.Grouping && x.Type == info.Type && x.Message == info.Message);
                if (existing != null)
                {
                    existing.RepeatCount++;
                    return existing;
                }
            }

            Messages.Add(info);
            return info;
        }

        public Task CloseAsync(MessageInfo info)
        {
            if (info == null || !Messages.Contains(info))
            {
                return Task.CompletedTask;
            }

            Messages.Remove(info);
            return info.OnClose == null ? Task.CompletedTask : info.OnClose();
        }

        public void CloseAll()
        {
            Messages.Clear();
        }
    }
}

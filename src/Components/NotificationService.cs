using System;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace Element
{
    public class NotificationService
    {
        internal ObservableCollection<NotificationOption> Notifications { get; set; }

        public NotificationService()
            : this(null)
        {
        }

        internal NotificationService(NotificationService inner)
        {
            Notifications = inner?.Notifications ?? new ObservableCollection<NotificationOption>();
        }

        public NotificationOption Show(string message)
        {
            return Show(new NotificationOption { Message = message });
        }

        public NotificationOption Show(string title, string message)
        {
            return Show(new NotificationOption { Title = title, Message = message });
        }

        public NotificationOption Show(string title, string message, MessageType type)
        {
            return Show(new NotificationOption { Title = title, Message = message, Type = type });
        }

        public NotificationOption Success(string title, string message)
        {
            return Show(title, message, MessageType.Success);
        }

        public NotificationOption Warning(string title, string message)
        {
            return Show(title, message, MessageType.Warning);
        }

        public NotificationOption Error(string title, string message)
        {
            return Show(title, message, MessageType.Error);
        }

        public NotificationOption Info(string title, string message)
        {
            return Show(title, message, MessageType.Info);
        }

        public NotificationOption Show(NotificationOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            Notifications.Add(option);
            return option;
        }

        public async Task CloseAsync(NotificationOption option)
        {
            if (option == null || !Notifications.Contains(option))
            {
                return;
            }

            Notifications.Remove(option);
            if (option.OnClose != null)
            {
                await option.OnClose();
            }
        }

        public void CloseAll()
        {
            Notifications.Clear();
        }
    }
}

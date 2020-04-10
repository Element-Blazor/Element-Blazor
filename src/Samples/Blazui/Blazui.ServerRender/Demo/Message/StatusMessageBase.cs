
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component;

namespace Blazui.ServerRender.Demo.Message
{
    public class StatusMessageBase : ComponentBase
    {
        [Inject]
        public MessageService Message { get; set; }
        
        public void ShowSuccessMessage()
        {
            Message.Show("成功消息", MessageType.Success);
        }
        public void ShowInfoMessage()
        {
            Message.Show("普通消息", MessageType.Info);
        }
        public void ShowWarningMessage()
        {
            Message.Show("警告消息", MessageType.Warning);
        }
        public void ShowErrorMessage()
        {
            Message.Show("错误消息", MessageType.Error);
        }
    }
}

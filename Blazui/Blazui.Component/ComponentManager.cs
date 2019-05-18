using Blazui.Component.Popup;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class ComponentManager
    {
        private static int zindex = 2000;
        private static Stack<BPopupBase> popupComponents = new Stack<BPopupBase>();
        public static async Task RegisterPopupComponentAsync(BPopupBase popupComponent)
        {
            if (popupComponents.Contains(popupComponent))
            {
                return;
            }
            popupComponent.OnDispose += PopupComponent_OnDispose;
            popupComponent.ZIndex = zindex++;
            popupComponents.Push(popupComponent);
            await Task.CompletedTask;
        }

        private static void PopupComponent_OnDispose()
        {
            popupComponents.Pop();
        }

        [JSInvokable]
        public static async Task OnBodyMouseUp()
        {
            if (!popupComponents.Any())
            {
                return;
            }
            var component = popupComponents.Peek();
            await component.HideAsync();
        }
    }
}

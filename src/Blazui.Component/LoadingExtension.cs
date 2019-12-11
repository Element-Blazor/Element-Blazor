using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public static class LoadingExtension
    {
        public static void Loading(this IContainerComponent container, LoadingService loadingService)
        {
            EnsureComponentStatus(container);
            loadingService.Show(new LoadingOption()
            {
                Target = container.Container
            });
        }
        public static void Loading(this IContainerComponent container, LoadingService loadingService, string text)
        {
            EnsureComponentStatus(container);
            loadingService.Show(new LoadingOption()
            {
                Text = text,
                Target = container.Container
            });
        }
        public static void Loading(this IContainerComponent container, LoadingService loadingService, string text, string iconClass, string background)
        {
            EnsureComponentStatus(container);
            loadingService.Show(new LoadingOption()
            {
                Text = text,
                Target = container.Container,
                IconClass = iconClass,
                Background = background
            });
        }

        public static void Close(this IContainerComponent container, LoadingService loadingService)
        {
            EnsureComponentStatus(container);
            var option = loadingService.LoadingOptions.FirstOrDefault(x => x.Target.Id == container.Container.Id);
            if (option == null)
            {
                return;
            }
            loadingService.LoadingOptions.Remove(option);
        }
        static void EnsureComponentStatus(IContainerComponent container)
        {
            if (container == null)
            {
                throw new BlazuiException("要置为 Loading 状态的组件尚未渲染完成");
            }
            if (string.IsNullOrWhiteSpace(container.Container.Id))
            {
                throw new BlazuiException("要置为 Loading 状态的组件尚未渲染完成，IContainerComponent.Container 的 Id 为 null");
            }
        }
    }
}

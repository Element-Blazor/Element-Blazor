using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public static class LoadingExtension
    {
        /// <summary>
        /// 显示 Loading 状态
        /// </summary>
        /// <param name="container"></param>
        /// <returns></returns>
        public static void Loading(this IContainerComponent container)
        {
            EnsureComponentStatus(container);
            container.LoadingService.Show(new LoadingOption()
            {
                Target = container.Container
            });
        }

        /// <summary>
        /// 显示 Loading 状态
        /// </summary>
        /// <param name="container"></param>
        /// <param name="action">在 Loading 状态期间要做的事</param>
        /// <returns></returns>
        public static async Task WithLoadingAsync(this IContainerComponent container, Func<Task> action)
        {
            EnsureComponentStatus(container);
            container.LoadingService.Show(new LoadingOption()
            {
                Target = container.Container
            });
            try
            {
                await action();
            }
            finally
            {
                container.LoadingService.Close();
            }
        }

        /// <summary>
        /// 显示 Loading 状态
        /// </summary>
        /// <param name="container"></param>
        /// <param name="action">在 Loading 状态期间要做的事</param>
        /// <param name="text"></param>
        /// <returns></returns>
        public static async Task WithLoadingAsync(this IContainerComponent container, Func<Task> action, string text)
        {
            EnsureComponentStatus(container);
            container.LoadingService.Show(new LoadingOption()
            {
                Text = text,
                Target = container.Container
            });
            try
            {
                await action();
            }
            finally
            {
                container.LoadingService.Close();
            }
        }
        public static async Task WithLoadingAsync(this IContainerComponent container, Func<Task> action, string text, string iconClass, string background)
        {
            EnsureComponentStatus(container);
            container.LoadingService.Show(new LoadingOption()
            {
                Text = text,
                Target = container.Container,
                IconClass = iconClass,
                Background = background
            });
            try
            {
                await action();
            }
            finally
            {
                container.LoadingService.Close();
            }
        }

        public static void Close(this IContainerComponent container)
        {
            EnsureComponentStatus(container);
            var option = container.LoadingService.LoadingOptions.FirstOrDefault(x => x.Target.Id == container.Container.Id);
            if (option == null)
            {
                return;
            }
            container.LoadingService.LoadingOptions.Remove(option);
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

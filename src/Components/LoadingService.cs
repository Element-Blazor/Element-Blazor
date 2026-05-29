using Microsoft.AspNetCore.Components;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class LoadingService
    {
        internal ObservableCollection<LoadingOption> LoadingOptions = new ObservableCollection<LoadingOption>();

        public LoadingOption Show(LoadingOption option)
        {
            if (option == null)
            {
                throw new ArgumentNullException(nameof(option));
            }

            if (!string.IsNullOrWhiteSpace(option.Target.Id) && LoadingOptions.Any(x => x.Target.Id == option.Target.Id))
            {
                return LoadingOptions.First(x => x.Target.Id == option.Target.Id);
            }

            LoadingOptions.Add(option);
            return option;
        }

        public LoadingOption Show()
        {
            return Show(new LoadingOption
            {
                Fullscreen = true
            });
        }

        public LoadingOption Show(string text)
        {
            return Show(new LoadingOption
            {
                Text = text,
                Fullscreen = true
            });
        }

        public LoadingOption ShowFullscreen(string text = null, string iconClass = null, string background = null, bool lockScroll = true)
        {
            return Show(new LoadingOption
            {
                Text = text,
                IconClass = iconClass,
                Background = background,
                Lock = lockScroll,
                Fullscreen = true
            });
        }

        public LoadingOption Show(ElementReference target, string text = null, string iconClass = null, string background = null)
        {
            return Show(new LoadingOption
            {
                Target = target,
                Text = text,
                IconClass = iconClass,
                Background = background
            });
        }

        public void Close(string targetId)
        {
            var option = LoadingOptions.FirstOrDefault(x => x.Target.Id == targetId);
            if (option != null)
            {
                LoadingOptions.Remove(option);
            }
        }

        public void Close(LoadingOption option)
        {
            if (option != null && LoadingOptions.Contains(option))
            {
                LoadingOptions.Remove(option);
            }
        }

        public void CloseFullScreenLoading()
        {
            var option = LoadingOptions.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.Target.Id));
            if (option != null)
            {
                LoadingOptions.Remove(option);
            }
        }

        public async Task WithLoadingAsync(Func<Task> action, string text = null, string iconClass = null, string background = null)
        {
            var option = ShowFullscreen(text, iconClass, background);
            try
            {
                await action();
            }
            finally
            {
                Close(option);
            }
        }
    }
}

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class LoadingService
    {
        internal ObservableCollection<LoadingOption> LoadingOptions = new ObservableCollection<LoadingOption>();

        public void Show(LoadingOption option)
        {
            if (LoadingOptions.Any(x => x.Target.Id == option.Target.Id))
            {
                return;
            }
            LoadingOptions.Add(option);
        }

        public void Show()
        {
            Show(new LoadingOption()
            {
            });
        }

        public void Close(string targetId)
        {
            var option = LoadingOptions.FirstOrDefault(x => x.Target.Id == targetId);
            LoadingOptions.Remove(option);
        }

        public void CloseFullScreenLoading()
        {
            var option = LoadingOptions.FirstOrDefault(x => string.IsNullOrWhiteSpace(x.Target.Id));
            LoadingOptions.Remove(option);
        }

        public void Show(string text)
        {
            Show(new LoadingOption()
            {
                Text = text
            });
        }
    }
}

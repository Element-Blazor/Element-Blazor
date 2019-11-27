using Blazui.Component;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Loading
{
    public class ServiceLoadingBase : ComponentBase
    {
        [Inject]
        private LoadingService loadingService { get; set; }
        public async Task ShowLoading()
        {
            loadingService.Show();
            await Task.Delay(2000);
            loadingService.Close();
        }
    }
}

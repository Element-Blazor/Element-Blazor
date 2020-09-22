
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using Element;
using System.Threading.Tasks;

namespace Element.ServerRender.Demo.Loading
{
    public class ServiceLoadingBase : ComponentBase
    {
        [Inject]
        private LoadingService loadingService { get; set; }
        public async Task ShowLoading()
        {
            loadingService.Show();
            await Task.Delay(2000);
            loadingService.CloseFullScreenLoading();
        }
    }
}

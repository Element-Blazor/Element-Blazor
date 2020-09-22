

using Element.ClientRender.PWA.Demo.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Element.ClientRender.PWA.Demo.Loading
{
    public class ManualLoadingBase : ComponentBase
    {
        protected List<TestData> Datas = new List<TestData>();

        [Inject]
        private LoadingService LoadingService { get; set; }
        protected IContainerComponent table;
        protected override void OnInitialized()
        {
            Datas.Add(new TestData()
            {
                Address = "地址1",
                Name = "张三",
                Time = DateTime.Now
            });
            Datas.Add(new TestData()
            {
                Address = "地址2",
                Name = "张三1",
                Time = DateTime.Now
            });
            Datas.Add(new TestData()
            {
                Address = "地址3",
                Name = "张三3",
                Time = DateTime.Now
            });
        }

        
        protected void ShowLoading(object arg)
        {
            table.Loading();
        }

        protected void CloseLoading()
        {
            table.Close();
        }
    }
}

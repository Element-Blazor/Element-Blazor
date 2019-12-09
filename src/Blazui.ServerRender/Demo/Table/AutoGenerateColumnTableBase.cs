using Blazui.Component;
using Blazui.Component.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Table
{
    public class AutoGenerateColumnTableBase : ComponentBase
    {
        protected List<AutoGenerateColumnTestData> Datas = new List<AutoGenerateColumnTestData>();

        [Inject]
        MessageService MessageService { get; set; }

        protected override void OnInitialized()
        {
            Datas.Add(new AutoGenerateColumnTestData()
            {
                Address = "地址1",
                Name = "张三",
                Time = DateTime.Now
            });
            Datas.Add(new AutoGenerateColumnTestData()
            {
                Address = "地址2",
                Name = "张三1",
                Time = DateTime.Now
            });
            Datas.Add(new AutoGenerateColumnTestData()
            {
                Address = "地址3",
                Name = "张三3",
                Time = DateTime.Now,
                Yes = true
            });
        }

        internal async Task<PagerResult<AutoGenerateColumnTestData>> LoadDataSource(int currentPage)
        {
            var result= new PagerResult<AutoGenerateColumnTestData>()
            {
                Rows = Datas,
                Total = Datas.Count
            };
            return await Task.FromResult(result);
        }
        public void Edit(AutoGenerateColumnTestData testData)
        {
            MessageService.Show($"正在编辑 " + testData.Name);
        }
        public void Del(AutoGenerateColumnTestData testData)
        {
            MessageService.Show($"正在删除 " + testData.Name, MessageType.Warning);
        }
    }
}

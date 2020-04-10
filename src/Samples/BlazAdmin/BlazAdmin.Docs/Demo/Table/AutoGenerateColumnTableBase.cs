using Blazui.Component;
using Blazui.Component.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazAdmin.Docs.Demo.Table
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

        internal async Task<PagerResult> LoadDataSource(int currentPage)
        {
            var result= new PagerResult()
            {
                Rows = Datas,
                Total = Datas.Count
            };
            return await Task.FromResult(result);
        }
        public void Edit(object testData)
        {
            MessageService.Show($"正在编辑 " + ((AutoGenerateColumnTestData)testData).Name);
        }
        public void Del(object testData)
        {
            MessageService.Show($"正在删除 " + ((AutoGenerateColumnTestData)testData).Name, MessageType.Warning);
        }
    }
}

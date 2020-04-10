using Blazui.Component;
using Blazui.Component.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazAdmin.Docs.Demo.Table
{
    public class PaginationTableBase : ComponentBase
    {
        protected List<AutoGenerateColumnTestData> AllDatas = new List<AutoGenerateColumnTestData>();

        protected BTable table;
        protected int currentPage = 1;

        protected int pageSize = 5;
        [Inject]
        MessageService MessageService { get; set; }

        protected override void OnInitialized()
        {
            for (int i = 0; i < 1000; i++)
            {
                AllDatas.Add(new AutoGenerateColumnTestData()
                {
                    Address = "地址" + i,
                    Name = "张三" + i,
                    Time = DateTime.Now
                });
            }
        }

        internal async Task<PagerResult> LoadDataSource(int currentPage)
        {
            var result= new PagerResult()
            {
                Rows = AllDatas.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList(),
                Total = AllDatas.Count
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

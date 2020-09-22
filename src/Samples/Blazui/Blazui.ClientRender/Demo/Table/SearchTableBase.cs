

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Element.ClientRender.Demo.Table
{
    public class SearchTableBase : ComponentBase
    {
        internal BForm searchForm;
        internal BTable table;
        protected int currentPage;
        private SearchCondition condition;
        protected List<AutoGenerateColumnTestData> AllDatas = new List<AutoGenerateColumnTestData>();
        protected List<AutoGenerateColumnTestData> Datas = new List<AutoGenerateColumnTestData>();

        protected override void OnInitialized()
        {
            for (int i = 0; i < 10; i++)
            {
                AllDatas.Add(new AutoGenerateColumnTestData()
                {
                    Address = "地址" + i,
                    Name = "张三" + i,
                    Time = DateTime.Now
                });
            }
            Datas = AllDatas;
        }
        internal Task SubmitAsync() => Task.Run(Submit);
        internal void Submit()
        {
            condition = searchForm.GetValue<SearchCondition>();
            Datas = AllDatas.Where(x => x.Name.Contains(condition.Name)).ToList();
            table.MarkAsRequireRender();
        }
    }
}

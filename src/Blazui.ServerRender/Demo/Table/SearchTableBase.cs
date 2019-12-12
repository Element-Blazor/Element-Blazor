using Blazui.Component.Form;
using Blazui.Component.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Table
{
    public class SearchTableBase : ComponentBase
    {
        internal BForm searchForm;
        internal BTable table;
        protected int currentPage;
        private SearchCondition condition;
        protected List<AutoGenerateColumnTestData> Datas = new List<AutoGenerateColumnTestData>();

        protected override void OnInitialized()
        {
            for (int i = 0; i < 10; i++)
            {
                Datas.Add(new AutoGenerateColumnTestData()
                {
                    Address = "地址" + i,
                    Name = "张三" + i,
                    Time = DateTime.Now
                });
            }
        }

        internal async Task Submit()
        {
            condition = searchForm.GetValue<SearchCondition>();
            table.Bind();
        }

        internal async Task<PagerResult> LoadDataSource(int currentPage)
        {
            var rows = Datas;
            if (condition != null)
            {
                rows = rows.Where(x => x.Name.Contains(condition.Name)).ToList();
            }
            var result = new PagerResult()
            {
                Rows = rows,
                Total = rows.Count
            };
            return await Task.FromResult(result);
        }
    }
}

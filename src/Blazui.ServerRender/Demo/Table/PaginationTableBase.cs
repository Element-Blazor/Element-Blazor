using Blazui.Component;
using Blazui.Component.Table;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Table
{
    public class PaginationTableBase : ComponentBase
    {
        protected List<AutoGenerateColumnTestData> AllDatas = new List<AutoGenerateColumnTestData>();
        protected List<AutoGenerateColumnTestData> Datas = new List<AutoGenerateColumnTestData>();

        protected BTable<AutoGenerateColumnTestData> table;
        protected int currentPage = 1;

        protected int pageSize = 5;
        internal int CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                currentPage = value;
                Datas = AllDatas.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            }
        }
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
            Datas = AllDatas.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
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

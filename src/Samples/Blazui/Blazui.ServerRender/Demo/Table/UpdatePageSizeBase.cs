using Element;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.ServerRender.Demo.Table
{
    public class UpdatePageSizeBase : ElementComponentBase
    {
        protected List<AutoGenerateColumnTestData> AllDatas = new List<AutoGenerateColumnTestData>();
        protected List<AutoGenerateColumnTestData> Datas = new List<AutoGenerateColumnTestData>();

        protected BTable table;
        protected int currentPage = 1;

        internal int CurrentPage
        {
            get
            {
                return currentPage;
            }
            set
            {
                currentPage = value;
                MarkAsRequireRender();
                Datas = AllDatas.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
            }
        }
        protected int pageSize = 5;

        protected int PageSize
        {
            get
            {
                return pageSize;
            }
            set
            {
                pageSize = value;
                Datas = AllDatas.Skip((currentPage - 1) * pageSize).Take(pageSize).ToList();
                MarkAsRequireRender();
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

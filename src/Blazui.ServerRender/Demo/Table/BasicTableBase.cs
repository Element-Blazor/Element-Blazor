using Blazui.Component;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Table
{
    public class BasicTableBase : ComponentBase
    {
        protected List<TestData> Datas = new List<TestData>();
        protected List<TestData> LargeDatas = new List<TestData>();

        [Inject]
        MessageService MessageService { get; set; }

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
            LargeDatas.AddRange(Datas);
            LargeDatas.AddRange(Datas);
            LargeDatas.AddRange(Datas);
            LargeDatas.AddRange(Datas);
            LargeDatas.AddRange(Datas);
        }

        public void Edit(TestData testData)
        {
            MessageService.Show($"正在编辑 " + testData.Name);
        }
        public void Del(TestData testData)
        {
            MessageService.Show($"正在删除 " + testData.Name, MessageType.Warning);
        }
    }
}

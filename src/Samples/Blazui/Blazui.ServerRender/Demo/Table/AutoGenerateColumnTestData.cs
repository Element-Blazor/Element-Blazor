
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Element;

namespace Element.ServerRender.Demo.Table
{
    public class AutoGenerateColumnTestData
    {
        [TableColumn(Text = "时间", Format = "yyyy-MM-dd")]
        public DateTime Time { get; set; }
        [TableColumn(Text = "姓名")]
        public string Name { get; set; }
        [TableColumn(Text = "地址")]
        public string Address { get; set; }

        [TableColumn(Text = "是/否")]
        public bool Yes { get; set; }
    }
}

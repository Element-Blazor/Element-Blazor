using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

namespace BlazAdmin.Docs.Demo.Table
{
    public class AutoGenerateColumnTestData
    {
        [Description("时间")]
        public DateTime Time { get; set; }
        [Description("姓名")]
        public string Name { get; set; }
        [Description("地址")]
        public string Address { get; set; }

        [Description("是/否")]
        public bool Yes { get; set; }
    }
}

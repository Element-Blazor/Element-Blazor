using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ClientRender.Model
{
    public class DemoPageModel
    {
        /// <summary>
        /// 示例页面名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 该页面所有示例
        /// </summary>
        public IEnumerable<DemoInfoModel> Demos { get; set; }
    }
}

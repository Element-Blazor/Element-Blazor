using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Client.Model
{
    public class DemoModel
    {
        public string Title { get; set; }
        public IList<CodeModel> Codes { get; set; } = new List<CodeModel>();
        public Type Demo { get; set; }

        /// <summary>
        /// 示例类型
        /// </summary>
        public string Type { get; set; }
    }
}

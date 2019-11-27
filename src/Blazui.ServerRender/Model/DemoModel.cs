using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Model
{
    public class DemoModel
    {
        public string Title { get; set; }
        public ObservableCollection<CodeModel> Codes { get; set; } = new ObservableCollection<CodeModel>();
        public Type Demo { get; set; }

        /// <summary>
        /// 示例类型
        /// </summary>
        public string Type { get; set; }
    }
}

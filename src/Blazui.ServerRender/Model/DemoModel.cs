
using Blazui.Component;
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
        public ObservableCollection<TabOption> Options { get; set; } = new ObservableCollection<TabOption>();
        public Type Demo { get; set; }

        /// <summary>
        /// 示例类型
        /// </summary>
        public string Type { get; set; }
    }
}

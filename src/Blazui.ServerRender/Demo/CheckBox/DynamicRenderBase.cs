using Blazui.Component.CheckBox;
using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.CheckBox
{
    public class DynamicRenderBase : ComponentBase
    {
        public List<string> Values { get; set; }
        public ObservableCollection<string> SelectedValues { get; set; }

        protected override void OnInitialized()
        {
            Values = new List<string>()
{
                    "列表选项1",
                    "列表选项2",
                    "列表选项3"
                };
            SelectedValues = new ObservableCollection<string>()
{
                "列表选项1",
                "列表选项3"
            };
        }
    }
}

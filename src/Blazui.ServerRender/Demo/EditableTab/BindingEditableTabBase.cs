using Blazui.Component.Container;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.EditableTab
{
    public class BindingEditableTabBase : ComponentBase
    {
        protected BTabPanel<TabOption> tabPanel;
        protected ObservableCollection<TabOption> models = new ObservableCollection<TabOption>()
{
        new TabOption
        {
            Name="tab1",
             Title="选项卡1",
             Content="内容1"
        },
        new TabOption
        {
            Name="tab2",
            Title="卡2",
             Content="内容2"
        },
        new TabOption
        {
            Name="tab3",
            Title="卡3",
             Content="内容3",
             IsClosable=true
        },
        new TabOption
        {
            Name="tab4",
            Title="Component",
             Content=typeof(TestCheckBoxInTab)
        }
    };
        protected void OnAddingTabAsync()
        {
            models.Add(new TabOption()
            {
                Content = "内容" + models.Count,
                IsClosable = true,
                Name = "name" + models.Count,
                Title = "标题" + models.Count
            });
        }
    }
}

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Container
{
    public interface ITab
    {
        RenderFragment ChildContent { get; set; }
        ElementRef Element { get; set; }
        bool IsActive { get; set; }
        Func<ITab, Task> OnRenderCompletedAsync { get; set; }
        BSimpleTab TabContainer { get; }
        string Title { get; set; }
        string Name { get; set; }
    }
}

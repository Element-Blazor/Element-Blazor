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
        ElementReference Element { get; set; }
        event Func<ITab, Task> OnRenderCompletedAsync;
        BSimpleTab TabContainer { get; }
        string Title { get; set; }
        string Name { get; set; }
    }
}

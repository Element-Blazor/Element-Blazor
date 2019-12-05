using Blazui.Component.NavMenu;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class SubMenuOption : PopupOption
    {
        public SubMenuOption()
        {
            IsShow = true;
        }
        public BSubMenuBase SubMenu { get; set; }
        public MenuOptions Options { get; set; }
        public RenderFragment Content { get; set; }
        public TaskCompletionSource<int> TaskCompletionSource { get; set; }
        public Func<SubMenuOption, Task> Close { get; set; }

        public CancellationTokenSource ClosingTaskCancellationTokenSource { get; set; }
        public Task ClosingTask { get; internal set; }
    }
}

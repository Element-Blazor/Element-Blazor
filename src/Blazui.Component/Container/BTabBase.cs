using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component.Dom;
using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;

namespace Blazui.Component.Container
{
    public class BTabBase : ComponentBase
    {
        [Parameter]
        public bool? IsClosable { get; set; }
        [Parameter]
        public bool? IsAddable { get; set; }

        [Parameter]
        public TabType Type { get; set; }

        [Parameter]
        public bool IsEditable { get; set; }

        [Parameter]
        public TabPosition TabPosition { get; set; }

        [Parameter]
        public string ActiveTabName { get; set; }

        [Inject]
        private IJSRuntime JSRuntime { get; set; }
        [Parameter]
        public RenderFragment ChildContent { get; set; }
        [Parameter]
        public EventCallback<BChangeEventArgs<ITab>> OnActiveTabChanged { get; set; }

        [Parameter]
        public Func<ITab, Task<bool>> OnActiveTabChangingAsync { get; set; }

        protected async Task<bool> OnInternalActiveTabChangingAsync(ITab tab)
        {
            if (OnActiveTabChangingAsync == null)
            {
                return await Task.FromResult(true);
            }
            return await OnActiveTabChangingAsync(tab);
        }

        public void Refresh()
        {
            StateHasChanged();
        }

        [Parameter]
        public EventCallback<MouseEventArgs> OnAddingTab { get; set; }
    }
}

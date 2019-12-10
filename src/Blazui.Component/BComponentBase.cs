using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BComponentBase : ComponentBase
    {
        [Inject]
        MessageBox MessageBox { get; set; }

        [Inject]
        public DialogService DialogService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        MessageService MessageService { get; set; }
        [Parameter]
        public string Style { get; set; } = string.Empty;

        public void Alert(string text)
        {
            _ = MessageBox.AlertAsync(text);
        }
        public void Toast(string text)
        {
            MessageService.Show(text);
        }
        public async Task<MessageBoxResult> AlertAsync(string text)
        {
            return await MessageBox.AlertAsync(text);
        }

        public void Refresh()
        {
            StateHasChanged();
        }
    }
}

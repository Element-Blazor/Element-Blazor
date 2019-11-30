using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
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
        [Parameter]
        public string Style { get; set; } = string.Empty;

        public void Alert(string text)
        {
            _ = MessageBox.AlertAsync(text);
        }
        public async Task<MessageBoxResult> AlertAsync(string text)
        {
            return await MessageBox.AlertAsync(text);
        }
    }
}

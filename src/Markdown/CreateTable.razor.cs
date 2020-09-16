using Blazui.Component;

using Blazui.Markdown.IconHandlers;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Markdown
{
    public partial class CreateTable
    {
        internal BForm form;

        internal void Submit()
        {
            if (!form.IsValid())
            {
                return;
            }

            var model = form.GetValue<CreateTableModel>();
            _ = DialogService.CloseDialogAsync(this, model);
        }
    }
}

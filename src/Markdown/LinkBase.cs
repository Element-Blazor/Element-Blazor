using Element;

using Element.Markdown.IconHandlers;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Element.Markdown
{
    public class LinkBase : BDialogBase
    {
        internal protected BForm form;

        [Parameter]
        public LinkModel Link { get; set; }

        internal protected void Submit()
        {
            if (!form.IsValid())
            {
                return;
            }
            _ = DialogService.CloseDialogAsync(this, form.GetValue<LinkModel>());
        }
    }
}

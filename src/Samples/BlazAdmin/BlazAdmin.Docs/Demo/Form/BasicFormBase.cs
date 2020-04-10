using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazAdmin.Docs.Demo.Form
{
    public class BasicFormBase : ComponentBase
    {
        internal LabelAlign formAlign;
        [Inject]
        Blazui.Component.MessageBox MessageBox { get; set; }

        protected BForm demoForm;
        protected void Submit()
        {
            if (!demoForm.IsValid())
            {
                return;
            }

            var activity = demoForm.GetValue<Activity>();
            _ = MessageBox.AlertAsync(activity.ToString());
        }

        

        protected void Reset()
        {
            demoForm.Reset();
        }
    }
}

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Form
{
    public class BFieldComponentBase<TValue> : BComponentBase, IDisposable
    {
        [CascadingParameter]
        public BFormItemBase<TValue> FormItem { get; set; }

        protected void SetFieldValue(TValue value)
        {
            if (FormItem == null)
            {
                return;
            }
            FormItem.Value = value;
            FormItem.Validate();
        }

        protected override void OnInitialized()
        {
            if (FormItem != null)
            {
                FormItem.OnReset += FormItem_OnReset;
            }
        }

        protected virtual void FormItem_OnReset()
        {

        }

        public void Dispose()
        {
            if (FormItem != null)
            {
                FormItem.OnReset -= FormItem_OnReset;
            }
        }
    }
}

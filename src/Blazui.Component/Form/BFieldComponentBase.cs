using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Form
{
    public class BFieldComponentBase<TValue> : BComponentBase, IDisposable
    {
        protected string Name { get; set; }
        [CascadingParameter]
        public BFormItemBase<TValue> FormItem { get; set; }

        protected void SetFieldValue(TValue value, bool validate)
        {
            if (FormItem == null)
            {
                return;
            }
            FormItem.Value = value;

            if (!validate)
            {
                return;
            }
            FormItem.Validate();
            FormItem.ShowErrorMessage();
        }

        protected override void OnInitialized()
        {
            if (FormItem != null)
            {
                FormItem.OnReset += FormItem_OnReset;
                Name = FormItem.Name;
            }
        }

        protected virtual void FormItem_OnReset(object value, bool requireRerender)
        {

        }

        public virtual void Dispose()
        {
            if (FormItem != null)
            {
                FormItem.OnReset -= FormItem_OnReset;
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (FormItem == null)
            {
                return;
            }
            if (!FormItem.OriginValueRendered)
            {
                FormItem.OriginValueRendered = true;
                FormItem_OnReset(FormItem.OriginValue, false);
            }
        }
    }
}

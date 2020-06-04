

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component;

namespace Blazui.ServerRender.Demo.Form
{
    public partial class AutoGenerateFieldsInitilizeForm
    {
        internal LabelAlign formAlign;
        [Inject]
        Component.MessageBox MessageBox { get; set; }

        internal object value;
        protected BForm demoForm;
        protected void Submit()
        {
            if (!demoForm.IsValid())
            {
                return;
            }

            var activity = demoForm.GetValue<AutoGenerateFieldsActvity>();
            _ = MessageBox.AlertAsync(activity.ToString());
        }

        protected override void OnInitialized()
        {
            value = new AutoGenerateFieldsActvity()
            {
                Area = Area.Shanghai,
                Delivery = true,
                Description = "详情",
                Name = "测试",
                Resource = "场地",
                Time = DateTime.Now
            };
        }

        protected void Reset()
        {
            demoForm.Reset();
        }
    }
}

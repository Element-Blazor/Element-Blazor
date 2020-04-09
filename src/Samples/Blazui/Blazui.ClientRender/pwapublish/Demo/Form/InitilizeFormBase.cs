

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component;

namespace Blazui.ClientRender.Demo.Form
{
    public class InitilizeFormBase : ComponentBase
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

            var activity = demoForm.GetValue<Activity>();
            _ = MessageBox.AlertAsync(activity.ToString());
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            value = new Activity()
            {
                Resource1 = Resource.Option2,
                Area = Area.Shanghai,
                Delivery = true,
                Description = "详情",
                Name = "测试",
                Resource = "场地",
                Time = DateTime.Now,
                Type = new List<string>()
                 {
                     "Offline","Online"
                 }
            };
        }

        protected void Reset()
        {
            demoForm.Reset();
        }
    }
}

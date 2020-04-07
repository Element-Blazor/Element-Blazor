using Blazui.Component.Form;
using Blazui.Component.Select;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BlazAdmin.Docs.Demo.Form
{
    public class AutoGenerateFieldsInitilizeFormBase : ComponentBase
    {
        internal LabelAlign formAlign;
        [Inject]
        Blazui.Component.MessageBox MessageBox { get; set; }

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

        protected override void OnInitialized()
        {
            value = new Activity()
            {
                Area = Area.Beijing,
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

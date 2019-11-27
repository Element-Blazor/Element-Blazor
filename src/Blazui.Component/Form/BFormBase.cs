using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Form
{
    public class BFormBase : BComponentBase, IContainerComponent
    {
        protected bool RequireRerender = true;
        public ElementReference Container { get; set; }

        internal List<BFormItemBaseObject> Items { get; set; } = new List<BFormItemBaseObject>();

        [Parameter]
        public bool Inline { get; set; }

        [Parameter]
        public LabelAlign LabelAlign { get; set; }

        [Parameter]
        public List<FormFieldValidation> Validations { get; set; } = new List<FormFieldValidation>();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        public T GetValue<T>()
        {
            if (!IsValid())
            {
                throw new BlazuiException("表单验证不通过，此时无法获取表单输入的值");
            }
            var value = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var formItem = Items.FirstOrDefault(x => x.Name == property.Name);
                if (formItem == null)
                {
                    continue;
                }

                object destValue = formItem.GetType().GetProperty("Value").GetValue(formItem);
                //if (property.PropertyType.IsGenericType)
                //{
                //    if (property.PropertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
                //    {
                //        var type = Nullable.GetUnderlyingType(property.PropertyType);
                //        if (formItem.Value != null && !string.IsNullOrWhiteSpace(formItem.Value.ToString()))
                //        {
                //            destValue = Convert.ChangeType(formItem.Value, type);
                //        }
                //    }
                //    else
                //    {
                //        destValue = Convert.ChangeType(formItem.Value, property.PropertyType);
                //    }
                //}
                //else
                //{
                //    destValue = Convert.ChangeType(formItem.Value, property.PropertyType);
                //}

                try
                {
                    property.SetValue(value, destValue);
                }
                catch (ArgumentException ex)
                {
                    throw new BlazuiException($"字段 {formItem.Name} 输入的类型为 {destValue.GetType()}，但实体 {typeof(T)} 对应的属性的类型为 {property.PropertyType}", ex);
                }
            }
            return value;
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (RequireRerender)
            {
                RequireRerender = false;
                StateHasChanged();
            }
        }

        public void Reset()
        {
            foreach (var item in Items)
            {
                item.Reset();
            }
        }

        public bool IsValid()
        {
            foreach (var item in Items)
            {
                item.Validate();
                item.IsShowing = true;
            }
            var isValid = Items.All(x => x.ValidationResult.IsValid);
            if (!isValid)
            {
                Task.Delay(10).ContinueWith((task) =>
                {
                    foreach (var item in Items)
                    {
                        item.IsShowing = false;
                    }
                    InvokeAsync(() =>
                    {
                        StateHasChanged();
                    });
                });
            }
            return Items.All(x => x.ValidationResult.IsValid);
        }
    }
}

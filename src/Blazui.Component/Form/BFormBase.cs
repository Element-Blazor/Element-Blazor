using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component.Form
{
    public class BFormBase : BComponentBase, IContainerComponent
    {
        /// <summary>
        /// 是否有初始值需要渲染
        /// </summary>
        internal bool OriginValueHasSet { get; set; } = false;
        private bool requireRefresh = true;
        private Task showMessageTask;
        public ElementReference Container { get; set; }

        internal List<BFormItemBaseObject> Items { get; set; } = new List<BFormItemBaseObject>();

        [Parameter]
        public bool Inline { get; set; }

        [Parameter]
        public LabelAlign LabelAlign { get; set; }

        /// <summary>
        /// 设置验证规则
        /// </summary>
        [Parameter]
        public List<FormFieldValidation> Validations { get; set; } = new List<FormFieldValidation>();

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 触发浏览器提交
        /// </summary>
        public async Task SubmitAsync(string url)
        {
            await Container.Dom(JSRuntime).SubmitAsync(url);
        }

        /// <summary>
        /// 该属性仅用于设置表单初始值，获取表单输入值请使用 <seealso cref="GetValue{T}"/> 方法
        /// </summary>
        [Parameter]
        public object Value { get; set; }

        public IDictionary<string, object> Values { get; set; } = new Dictionary<string, object>();
        /// <summary>
        /// 获取表单输入值
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
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

        private void SetValues()
        {
            if (Value == null)
            {
                return;
            }
            Values = Value.GetType().GetProperties().ToDictionary(x => x.Name, x => x.GetValue(Value));
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            SetValues();
        }

        internal void ShowErrorMessage()
        {
            if (showMessageTask != null)
            {
                return;
            }

            showMessageTask = Task.Delay(100).ContinueWith((task) =>
            {
                foreach (var item in Items)
                {
                    item.MarkAsRequireRender();
                    item.IsShowing = false;
                }
                RequireRender = true;
                InvokeAsync(StateHasChanged);
                showMessageTask = null;
            });
        }

        protected override void OnAfterRender(bool firstRender)
        {
            if (requireRefresh)
            {
                requireRefresh = false;
                RequireRender = true;
                StateHasChanged();
                return;
            }
        }

        public void Reset()
        {
            foreach (var item in Items)
            {
                item.MarkAsRequireRender();
                item.Reset();
            }
            RequireRender = true;
            StateHasChanged();
        }

        public bool IsValid()
        {
            RequireRender = true;
            foreach (var item in Items)
            {
                item.MarkAsRequireRender();
                item.Validate();
                item.IsShowing = true;
            }
            var isValid = Items.All(x => x.ValidationResult.IsValid);
            if (!isValid)
            {
                ShowErrorMessage();
            }
            return isValid;
        }
    }
}

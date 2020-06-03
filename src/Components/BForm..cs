using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BForm : BComponentBase, IContainerComponent
    {
        [Inject]
        FormFieldControlMap formFieldControlMap { get; set; }

        private bool requireRefresh = true;
        private Task showMessageTask;
        public ElementReference Container { get; set; }

        internal List<BFormItemObject> Items { get; set; } = new List<BFormItemObject>();

        [Parameter]
        public bool Inline { get; set; }

        /// <summary>
        /// 表单名称
        /// </summary>
        [Parameter]
        public string Name { get; set; }

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
        /// 表单按钮
        /// </summary>
        [Parameter]
        public RenderFragment Buttons { get; set; }

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


        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            var clsList = new List<string>();
            if (Inline)
            {
                clsList.Add("el-form--inline");
            }
            switch (LabelAlign)
            {
                case LabelAlign.Left:
                    clsList.Add("el-form--label-left");
                    break;
                case LabelAlign.Right:
                    clsList.Add("el-form--label-right");
                    break;
                case LabelAlign.Top:
                    clsList.Add("el-form--label-top");
                    break;
            }
            clsList.Add("el-form");

            builder.OpenElement(0, "form");
            builder.AddAttribute(1, "class", string.Join(" ", clsList));
            builder.AddAttribute(2, "style", Style);
            builder.AddElementReferenceCapture(3, (__value) => Container = __value);
            TypeInference.CreateCascadingValue_0(builder, 4, 5, this, 6, (__builder2) =>
             {
                 var index = 7;
                 if (EntityType != null)
                 {
                     var formItemConfigs = formFieldControlMap.GetFormItems(EntityType, Name);
                     ChildContent = formItemsBuilder =>
                     {
                         foreach (var formItemConfig in formItemConfigs.Values)
                         {
                             index = formItemConfig.Index + index;
                             formItemsBuilder.OpenComponent(++index, formItemConfig.FormItem);
                             formItemsBuilder.AddAttribute(++index, nameof(BFormItemObject.IsRequired), true);
                             formItemsBuilder.AddAttribute(++index, nameof(BFormItemObject.RequiredMessage), "xxxx");
                             formItemsBuilder.AddAttribute(++index, nameof(BFormItemObject.Label), formItemConfig.Label);
                             formItemsBuilder.AddAttribute(++index, nameof(BFormItemObject.Image), formItemConfig.Image);
                             formItemsBuilder.AddAttribute(++index, nameof(BFormItemObject.Name), formItemConfig.Name);
                             formItemsBuilder.AddAttribute(++index, nameof(BFormItemObject.LabelWidth), formItemConfig.LabelWidth);
                             formItemsBuilder.AddAttribute(++index, nameof(BFormItemObject.ChildContent), (RenderFragment)(inputControlBuilder =>
                             {
                                 inputControlBuilder.OpenComponent(++index, formItemConfig.InputControl);
                                 inputControlBuilder.AddAttribute(++index, nameof(BFormItemObject.EnableAlwaysRender), true);
                                 if (!string.IsNullOrWhiteSpace(formItemConfig.Placeholder))
                                 {
                                     inputControlBuilder.AddAttribute(++index, nameof(BFormItemObject.Placeholder), formItemConfig.Placeholder);
                                 }
                                 inputControlBuilder.CloseComponent();
                             }
                             ));
                             formItemsBuilder.CloseComponent();
                         }
                         if (Buttons != null)
                         {
                             formItemsBuilder.OpenComponent<BFormActionItem>(++index);
                             formItemsBuilder.AddAttribute(++index, nameof(BFormItemObject.Style), "text-align:center");
                             formItemsBuilder.AddAttribute(++index, nameof(BFormItemObject.ChildContent), Buttons);
                             formItemsBuilder.CloseComponent();
                         }
                     };
                 }
                 __builder2.AddContent(index, ChildContent);
             }
            );
            builder.CloseElement();
        }

        /// <summary>
        /// 设置后自动生成表单
        /// </summary>
        [Parameter]
        public Type EntityType { get; set; }

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


    internal static class TypeInference
    {
        public static void CreateCascadingValue_0<TValue>(global::Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder __builder, int seq, int __seq0, TValue __arg0, int __seq1, global::Microsoft.AspNetCore.Components.RenderFragment __arg1)
        {
            __builder.OpenComponent<global::Microsoft.AspNetCore.Components.CascadingValue<TValue>>(seq);
            __builder.AddAttribute(__seq0, "Value", __arg0);
            __builder.AddAttribute(__seq1, "ChildContent", __arg1);
            __builder.CloseComponent();
        }
    }
}

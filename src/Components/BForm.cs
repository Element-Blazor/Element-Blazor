using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BForm : BComponentBase, IContainerComponent
    {
        private List<FormItemConfig> formItemConfigs;
        private readonly List<FormInputRegistration> inputRegistrations = new List<FormInputRegistration>();
        [Inject]
        FormFieldControlMap formFieldControlMap { get; set; }

        private bool requireRefresh = true;
        private Task showMessageTask;
        private IDictionary<string, IList<IValidationRule>> lastRulesReference;
        public ElementReference Container { get; set; }

        internal List<BFormItemObject> Items { get; set; } = new List<BFormItemObject>();

        [Parameter]
        public bool Inline { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public InputSize? Size { get; set; }

        [Parameter]
        public bool InlineMessage { get; set; }

        [Parameter]
        public bool StatusIcon { get; set; }

        [Parameter]
        public bool ShowMessage { get; set; } = true;

        [Parameter]
        public bool ValidateOnRuleChange { get; set; } = true;

        [Parameter]
        public bool HideRequiredAsterisk { get; set; }

        [Parameter]
        public bool ScrollToError { get; set; }

        [Parameter]
        public IDictionary<string, IList<IValidationRule>> Rules { get; set; }

        [Parameter]
        public EventCallback<FormValidateEventArgs> OnValidate { get; set; }

        [Parameter]
        public object LabelPosition
        {
            get => LabelAlign.ToString().ToLower();
            set
            {
                if (value == null)
                {
                    return;
                }
                if (value is LabelAlign directAlign)
                {
                    LabelAlign = directAlign;
                    return;
                }
                if (Enum.TryParse<LabelAlign>(Convert.ToString(value), true, out var labelAlign))
                {
                    LabelAlign = labelAlign;
                }
            }
        }

        [Parameter]
        public object LabelWidth { get; set; } = string.Empty;

        [Parameter]
        public string LabelSuffix { get; set; } = string.Empty;

        [Parameter]
        public string RequireAsteriskPosition { get; set; } = "left";

        /// <summary>
        /// 是否是创建
        /// </summary>
        [Parameter]
        public bool IsCreate { get; set; } = true;

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

        [Parameter]
        public object Model
        {
            get => Value;
            set => Value = value;
        }


        protected override void BuildRenderTree(Microsoft.AspNetCore.Components.Rendering.RenderTreeBuilder builder)
        {
            var clsList = new List<string>();
            if (Inline)
            {
                clsList.Add("el-form--inline");
            }
            if (Disabled)
            {
                clsList.Add("is-disabled");
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
            if (Size != null)
            {
                var sizeCssValue = GetSizeCssValue(Size.Value);
                if (sizeCssValue != null)
                {
                    clsList.Add($"el-form--{sizeCssValue}");
                }
            }

            builder.OpenElement(0, "form");
            if (Attributes != null)
            {
                builder.AddMultipleAttributes(1, Attributes);
            }
            builder.AddAttribute(2, "class", string.Join(" ", clsList));
            builder.AddAttribute(3, "style", Style);
            builder.AddElementReferenceCapture(4, value => Container = value);
            TypeInference.CreateCascadingValue_0(builder, 5, 6, this, 7, (__builder2) =>
             {
                 if (EntityType != null)
                 {
                     if (formItemConfigs == null)
                     {
                         formItemConfigs = formFieldControlMap.GetFormItems(EntityType);
                     }
                     ChildContent = formItemsBuilder =>
                     {
                         foreach (var formItemConfig in formItemConfigs)
                         {
                             formItemConfig.Page = Page;
                             if (formItemConfig.Ignore)
                             {
                                 continue;
                             }
                             formItemsBuilder.OpenComponent(1, formItemConfig.FormItem);
                             formItemsBuilder.AddAttribute(2, nameof(BFormItemObject.IsRequired), formItemConfig.IsRequired);
                             formItemsBuilder.AddAttribute(3, nameof(BFormItemObject.RequiredMessage), formItemConfig.RequiredMessage);
                             formItemsBuilder.AddAttribute(4, nameof(BFormItemObject.Label), formItemConfig.Label);
                             formItemsBuilder.AddAttribute(5, nameof(BFormItemObject.Image), formItemConfig.Image);
                             formItemsBuilder.AddAttribute(6, nameof(BFormItemObject.Name), formItemConfig.Name);
                             formItemsBuilder.AddAttribute(7, nameof(BFormItemObject.LabelWidth), formItemConfig.LabelWidth);
                             formItemsBuilder.AddAttribute(8, nameof(BFormItemObject.ChildContent), (RenderFragment)(inputControlBuilder =>
                             {
                                 formItemConfig.InputControlRender.Render(inputControlBuilder, formItemConfig);
                             }
                             ));
                             formItemsBuilder.CloseComponent();
                         }
                         if (Buttons != null)
                         {
                             formItemsBuilder.OpenComponent<BFormActionItem>(11);
                             formItemsBuilder.AddAttribute(12, nameof(Style), "text-align:right");
                             formItemsBuilder.AddAttribute(13, nameof(BFormItemObject.ChildContent), Buttons);
                             formItemsBuilder.CloseComponent();
                         }
                     };
                 }
                 __builder2.AddContent(8, ChildContent);
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
            Values = Value.GetType().GetProperties().Reverse().ToDictionary(x => x.Name, x => x.GetValue(Value));
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (ValidateOnRuleChange && !ReferenceEquals(Rules, lastRulesReference))
            {
                ClearValidate();
            }
            lastRulesReference = Rules;
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

        public void ResetFields(params string[] props)
        {
            foreach (var item in FilterItems(props))
            {
                item.MarkAsRequireRender();
                item.Reset();
            }
            RequireRender = true;
            StateHasChanged();
        }

        public void ClearValidate(params string[] props)
        {
            foreach (var item in FilterItems(props))
            {
                item.ClearValidate();
            }
            RequireRender = true;
            StateHasChanged();
        }

        public BFormItemObject GetField(string prop)
        {
            return Items.FirstOrDefault(x => x.Name == prop);
        }

        public bool ValidateField(params string[] props)
        {
            var items = FilterItems(props).ToList();
            RequireRender = true;
            foreach (var item in items)
            {
                item.MarkAsRequireRender();
                item.Validate();
                item.IsShowing = true;
            }
            var isValid = items.All(x => x.ValidationResult == null || x.ValidationResult.IsValid);
            if (!isValid)
            {
                ShowErrorMessage();
            }
            if (ScrollToError)
            {
                _ = ScrollToFirstErrorAsync();
            }
            return isValid;
        }

        public async Task ScrollToFieldAsync(string prop)
        {
            var id = ResolveInputId(GetField(prop));
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }
            await JSRuntime.InvokeVoidAsync("scrollElementIntoViewById", id);
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
            if (ScrollToError)
            {
                _ = ScrollToFirstErrorAsync();
            }
            return isValid;
        }

        internal void RegisterInput(string prop, string id, object input)
        {
            if (string.IsNullOrWhiteSpace(prop) || string.IsNullOrWhiteSpace(id) || input == null)
            {
                return;
            }

            inputRegistrations.RemoveAll(x => ReferenceEquals(x.Input, input));
            inputRegistrations.Add(new FormInputRegistration(prop, id, input));
        }

        internal void UnregisterInput(object input)
        {
            inputRegistrations.RemoveAll(x => ReferenceEquals(x.Input, input));
        }

        internal string ResolveLabelWidth(object itemLabelWidth)
        {
            var width = itemLabelWidth ?? LabelWidth;
            if (width == null)
            {
                return string.Empty;
            }
            if (width is string stringWidth)
            {
                return stringWidth;
            }
            return $"{width}px";
        }

        internal string ResolveInputId(BFormItemObject item)
        {
            if (item == null)
            {
                return null;
            }
            if (!string.IsNullOrWhiteSpace(item.For))
            {
                return item.For;
            }

            if (string.IsNullOrWhiteSpace(item.Name))
            {
                return null;
            }

            return inputRegistrations.LastOrDefault(x => x.Prop == item.Name).Id;
        }

        private Task ScrollToFirstErrorAsync()
        {
            var firstErrorItem = Items.FirstOrDefault(x => x.ValidationResult != null && !x.ValidationResult.IsValid);
            if (firstErrorItem == null)
            {
                return Task.CompletedTask;
            }
            return ScrollToFieldAsync(firstErrorItem.Name);
        }

        private IEnumerable<BFormItemObject> FilterItems(params string[] props)
        {
            if (props == null || props.Length == 0 || props.All(string.IsNullOrWhiteSpace))
            {
                return Items;
            }

            return Items.Where(x => props.Contains(x.Name));
        }

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };

        private readonly struct FormInputRegistration
        {
            public FormInputRegistration(string prop, string id, object input)
            {
                Prop = prop;
                Id = id;
                Input = input;
            }

            public string Prop { get; }

            public string Id { get; }

            public object Input { get; }
        }
    }

    public class FormValidateEventArgs
    {
        public string Prop { get; set; }

        public bool IsValid { get; set; }

        public string Message { get; set; }
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

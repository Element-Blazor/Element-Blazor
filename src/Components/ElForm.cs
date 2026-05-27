using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElForm : ElementComponentBase, IContainerComponent
    {
        private List<FormItemConfig> formItemConfigs;
        private readonly List<FormInputRegistration> inputRegistrations = new List<FormInputRegistration>();
        [Inject]
        FormFieldControlMap formFieldControlMap { get; set; }

        private bool requireRefresh = true;
        private Task showMessageTask;
        private IDictionary<string, IList<IValidationRule>> lastRulesReference;
        private List<FormFieldValidation> lastValidationsReference;
        private object lastValueReference;
        private Type lastEntityTypeReference;
        private bool hasResolvedRules;
        private string labelPosition;
        public ElementReference Container { get; set; }

        [CascadingParameter]
        public EditContext CascadedEditContext { get; set; }

        internal List<ElFormItemObject> Items { get; set; } = new List<ElFormItemObject>();

        public IReadOnlyList<ElFormItemObject> Fields => Items;

        [Parameter]
        public bool Inline { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public InputSize? Size { get; set; }

        internal InputSize EffectiveSize => Size ?? ResolveInputSize(InputSize.Normal);

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
        public string ScrollIntoViewOptions { get; set; }

        [Parameter]
        public IDictionary<string, IList<IValidationRule>> Rules { get; set; }

        [Parameter]
        public EventCallback<FormValidateEventArgs> OnValidate { get; set; }

        [Parameter]
        public EventCallback OnSubmit { get; set; }

        [Parameter]
        public EventCallback OnValidSubmit { get; set; }

        [Parameter]
        public EventCallback OnInvalidSubmit { get; set; }

        [Parameter]
        public EventCallback<ElFormSubmitEventArgs> OnSubmitForm { get; set; }

        [Parameter]
        public EventCallback<ElFormSubmitEventArgs> OnValidSubmitForm { get; set; }

        [Parameter]
        public EventCallback<ElFormSubmitEventArgs> OnInvalidSubmitForm { get; set; }

        [Parameter]
        public object LabelPosition
        {
            get => string.IsNullOrWhiteSpace(labelPosition) ? NormalizeLabelPosition(LabelAlign) : labelPosition;
            set
            {
                if (value == null)
                {
                    return;
                }
                var stringValue = Convert.ToString(value);
                if (string.IsNullOrWhiteSpace(stringValue))
                {
                    return;
                }
                if (value is LabelAlign directAlign)
                {
                    LabelAlign = directAlign;
                    labelPosition = NormalizeLabelPosition(directAlign);
                    return;
                }
                if (Enum.TryParse<LabelAlign>(stringValue, true, out var labelAlign))
                {
                    LabelAlign = labelAlign;
                    labelPosition = NormalizeLabelPosition(labelAlign);
                    return;
                }
                labelPosition = stringValue.Trim().ToLowerInvariant();
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

        private async Task OnSubmitAsync()
        {
            var submitArgs = new ElFormSubmitEventArgs(this);
            if (OnSubmit.HasDelegate)
            {
                await OnSubmit.InvokeAsync();
            }

            if (OnSubmitForm.HasDelegate)
            {
                await OnSubmitForm.InvokeAsync(submitArgs);
            }

            if (!OnValidSubmit.HasDelegate
                && !OnInvalidSubmit.HasDelegate
                && !OnValidSubmitForm.HasDelegate
                && !OnInvalidSubmitForm.HasDelegate)
            {
                return;
            }

            var isValid = await ValidateAsync();
            submitArgs.IsValid = isValid;
            if (isValid)
            {
                await OnValidSubmit.InvokeAsync();
                await OnValidSubmitForm.InvokeAsync(submitArgs);
                return;
            }

            await OnInvalidSubmit.InvokeAsync();
            await OnInvalidSubmitForm.InvokeAsync(submitArgs);
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
            var effectiveSize = EffectiveSize;
            if (effectiveSize != InputSize.Normal)
            {
                var sizeCssValue = GetSizeCssValue(effectiveSize);
                if (sizeCssValue != null)
                {
                    clsList.Add($"el-form--{sizeCssValue}");
                }
            }

            builder.OpenElement(0, "form");
            var seq = 1;
            if (Attributes != null)
            {
                builder.AddMultipleAttributes(seq++, Attributes);
            }
            builder.AddAttribute(seq++, "class", string.Join(" ", clsList));
            builder.AddAttribute(seq++, "style", Style);
            if (!string.IsNullOrWhiteSpace(Name))
            {
                builder.AddAttribute(seq++, "name", Name);
            }
            builder.AddAttribute(seq++, "onsubmit", EventCallback.Factory.Create(this, OnSubmitAsync));
            builder.AddEventPreventDefaultAttribute(seq++, "onsubmit", true);
            builder.AddElementReferenceCapture(seq++, value => Container = value);
            TypeInference.CreateCascadingValue_0(builder, 7, 8, this, 9, (__builder2) =>
             {
                 if (EntityType != null)
                 {
                     if (formItemConfigs == null || EntityType != lastEntityTypeReference)
                     {
                         formItemConfigs = formFieldControlMap.GetFormItems(EntityType);
                         lastEntityTypeReference = EntityType;
                     }
                     ChildContent = formItemsBuilder =>
                     {
                         var itemSeq = 0;
                         foreach (var formItemConfig in formItemConfigs)
                         {
                             formItemConfig.Page = Page;
                             if (formItemConfig.Ignore)
                             {
                                 continue;
                             }
                             formItemsBuilder.OpenComponent(itemSeq++, formItemConfig.FormItem);
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(ElFormItemObject.IsRequired), formItemConfig.IsRequired);
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(ElFormItemObject.RequiredMessage), formItemConfig.RequiredMessage);
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(ElFormItemObject.Label), formItemConfig.Label);
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(ElFormItemObject.Image), formItemConfig.Image);
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(ElFormItemObject.Name), formItemConfig.Name);
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(ElFormItemObject.LabelWidth), formItemConfig.LabelWidth);
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(ElFormItemObject.ChildContent), (RenderFragment)(inputControlBuilder =>
                             {
                                 inputControlBuilder.OpenComponent<ElDynamicComponent>(0);
                                 inputControlBuilder.AddAttribute(1, nameof(ElDynamicComponent.Component), formItemConfig.InputControlRender);
                                 inputControlBuilder.AddAttribute(2, nameof(ElDynamicComponent.Config), formItemConfig);
                                 inputControlBuilder.CloseComponent();
                             }
                             ));
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(ElFormItemObject.EnableAlwaysRender), true);
                             formItemsBuilder.CloseComponent();
                         }
                         if (Buttons != null)
                         {
                             formItemsBuilder.OpenComponent<ElFormActionItem>(itemSeq++);
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(Style), "text-align:right");
                             formItemsBuilder.AddAttribute(itemSeq++, nameof(ElFormItemObject.ChildContent), Buttons);
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
                throw new ElementException("表单验证不通过，此时无法获取表单输入的值");
            }
            var value = Activator.CreateInstance<T>();
            var properties = typeof(T).GetProperties();
            foreach (var property in properties)
            {
                var formItem = Items.FirstOrDefault(x => string.Equals(x.Name, property.Name, StringComparison.OrdinalIgnoreCase));
                if (formItem == null)
                {
                    continue;
                }

                object destValue = formItem.CurrentValue;
                try
                {
                    property.SetValue(value, ConvertValue(destValue, property.PropertyType));
                }
                catch (Exception ex) when (ex is ArgumentException || ex is InvalidCastException || ex is FormatException)
                {
                    throw new ElementException($"字段 {formItem.Name} 输入的类型为 {destValue?.GetType().ToString() ?? "null"}，但实体 {typeof(T)} 对应的属性的类型为 {property.PropertyType}", ex);
                }
            }
            return value;
        }

        private void SetValues()
        {
            var model = GetModel();
            if (model == null)
            {
                Values = Items
                    .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                    .GroupBy(x => x.Name)
                    .ToDictionary(x => x.Key, x => x.Last().CurrentValue);
                return;
            }
            Values = Items
                .Where(x => !string.IsNullOrWhiteSpace(x.Name))
                .GroupBy(x => x.Name)
                .ToDictionary(x => x.Key, x => TryGetModelValue(x.Key, out var value) ? value : x.Last().CurrentValue);

            var modelValues = model.GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                .Where(x => x.CanRead)
                .Reverse()
                .ToDictionary(x => x.Name, x => x.GetValue(model));

            foreach (var item in modelValues)
            {
                Values[item.Key] = item.Value;
            }
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            var rulesChanged = !ReferenceEquals(Rules, lastRulesReference) || !ReferenceEquals(Validations, lastValidationsReference);
            var model = GetModel();
            var modelChanged = !ReferenceEquals(model, lastValueReference);
            SetValues();
            if (modelChanged)
            {
                RefreshFieldInitialValues(resetCurrentValue: true);
                lastValueReference = model;
            }
            if (rulesChanged && hasResolvedRules)
            {
                foreach (var item in Items)
                {
                    item.RefreshRules();
                }
                if (ValidateOnRuleChange)
                {
                    ValidateField();
                }
                else
                {
                    ClearValidate();
                }
            }

            lastRulesReference = Rules;
            lastValidationsReference = Validations;
            hasResolvedRules = true;
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
            ResetFields();
        }

        public void ResetFields()
        {
            foreach (var item in Items)
            {
                item.MarkAsRequireRender();
                item.Reset();
                NotifyFieldValueChanged(item, item.CurrentValue, validate: false);
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
                NotifyFieldValueChanged(item, item.CurrentValue, validate: false);
            }
            RequireRender = true;
            StateHasChanged();
        }

        public void ResetField(string prop)
        {
            ResetFields(prop);
        }

        public void ResetFields(IEnumerable<string> props)
        {
            ResetFields(props?.ToArray() ?? Array.Empty<string>());
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

        public void ClearValidate(IEnumerable<string> props)
        {
            ClearValidate(props?.ToArray() ?? Array.Empty<string>());
        }

        public ElFormItemObject GetField(string prop)
        {
            return Items.FirstOrDefault(x => string.Equals(x.Name, prop, StringComparison.OrdinalIgnoreCase));
        }

        public ElFormItemObject GetField(params string[] prop)
        {
            return FilterItems(prop).FirstOrDefault();
        }

        public IReadOnlyList<ElFormItemObject> GetFields(params string[] props)
        {
            return FilterItems(props).ToList();
        }

        public bool ValidateField(params string[] props)
        {
            return ValidateItems(FilterItems(props), scrollToError: ScrollToError);
        }

        public bool ValidateField(IEnumerable<string> props)
        {
            return ValidateField(props?.ToArray() ?? Array.Empty<string>());
        }

        public Task<bool> ValidateFieldAsync(params string[] props)
        {
            return ValidateItemsAsync(FilterItems(props), scrollToError: ScrollToError);
        }

        public Task<bool> ValidateFieldAsync(IEnumerable<string> props)
        {
            return ValidateFieldAsync(props?.ToArray() ?? Array.Empty<string>());
        }

        public async Task ScrollToFieldAsync(string prop)
        {
            var field = GetField(prop);
            var id = field?.ApplyStyle == true && !string.IsNullOrWhiteSpace(field.FieldId)
                ? field.FieldId
                : ResolveInputId(field);
            if (string.IsNullOrWhiteSpace(id))
            {
                return;
            }
            await JSRuntime.InvokeVoidAsync("scrollElementIntoViewById", id, ScrollIntoViewOptions);
        }

        public bool IsValid()
        {
            return Validate();
        }

        public bool Validate()
        {
            return ValidateItems(Items, scrollToError: ScrollToError);
        }

        public Task<bool> ValidateAsync()
        {
            return ValidateItemsAsync(Items, scrollToError: ScrollToError);
        }

        internal void RegisterInput(string prop, string id, object input, ElFormItemObject field = null)
        {
            if (string.IsNullOrWhiteSpace(prop) || string.IsNullOrWhiteSpace(id) || input == null)
            {
                return;
            }

            inputRegistrations.RemoveAll(x => ReferenceEquals(x.Input, input));
            inputRegistrations.Add(new FormInputRegistration(prop, id, input, field));
        }

        internal void UnregisterInput(object input)
        {
            inputRegistrations.RemoveAll(x => ReferenceEquals(x.Input, input));
        }

        internal void RegisterField(ElFormItemObject item)
        {
            if (item == null || Items.Contains(item))
            {
                return;
            }

            Items.Add(item);
            ApplyInitialValue(item, resetCurrentValue: false);
        }

        internal void UnregisterField(ElFormItemObject item)
        {
            if (item == null)
            {
                return;
            }

            Items.Remove(item);
            if (!string.IsNullOrWhiteSpace(item.Name))
            {
                inputRegistrations.RemoveAll(x => x.Prop == item.Name && (ReferenceEquals(x.Field, item) || x.Field == null));
            }
        }

        internal void NotifyFieldValueChanged(ElFormItemObject item, object value, bool validate = false)
        {
            if (item == null || string.IsNullOrWhiteSpace(item.Name))
            {
                return;
            }

            Values[item.Name] = value;
            if (validate)
            {
                item.MarkAsRequireRender();
                item.Validate();
                item.ShowErrorMessage();
            }
            var model = GetModel();
            if (model == null)
            {
                return;
            }

            if (FormModelPath.TrySetValue(model, item.Name, value, ConvertValue))
            {
                return;
            }
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
                return ElementCssUtility.NormalizeCssSize(stringWidth);
            }
            return $"{Convert.ToString(width, System.Globalization.CultureInfo.InvariantCulture)}px";
        }

        internal string ResolveInputId(ElFormItemObject item)
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

            return inputRegistrations.LastOrDefault(x => string.Equals(x.Prop, item.Name, StringComparison.OrdinalIgnoreCase) && ReferenceEquals(x.Field, item)).Id
                ?? inputRegistrations.LastOrDefault(x => string.Equals(x.Prop, item.Name, StringComparison.OrdinalIgnoreCase)).Id;
        }

        private bool ValidateItems(IEnumerable<ElFormItemObject> items, bool scrollToError)
        {
            var itemList = items.Where(x => x != null).ToList();
            RequireRender = true;
            foreach (var item in itemList)
            {
                item.MarkAsRequireRender();
                item.Validate();
                item.IsShowing = true;
            }
            var isValid = itemList.All(x => x.ValidationResult == null || x.ValidationResult.IsValid);
            if (!isValid)
            {
                ShowErrorMessage();
            }
            if (!isValid && scrollToError)
            {
                var firstErrorItem = itemList.FirstOrDefault(x => x.ValidationResult != null && !x.ValidationResult.IsValid);
                if (firstErrorItem != null)
                {
                    _ = ScrollToFieldAsync(firstErrorItem.Name);
                }
            }
            StateHasChanged();
            return isValid;
        }

        private async Task<bool> ValidateItemsAsync(IEnumerable<ElFormItemObject> items, bool scrollToError)
        {
            var itemList = items.Where(x => x != null).ToList();
            RequireRender = true;
            foreach (var item in itemList)
            {
                item.MarkAsRequireRender();
                await item.ValidateAsync();
                item.IsShowing = true;
            }
            var isValid = itemList.All(x => x.ValidationResult == null || x.ValidationResult.IsValid);
            if (!isValid)
            {
                ShowErrorMessage();
            }
            if (!isValid && scrollToError)
            {
                var firstErrorItem = itemList.FirstOrDefault(x => x.ValidationResult != null && !x.ValidationResult.IsValid);
                if (firstErrorItem != null)
                {
                    await ScrollToFieldAsync(firstErrorItem.Name);
                }
            }
            StateHasChanged();
            return isValid;
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

        private object GetModel()
        {
            return Value ?? CascadedEditContext?.Model;
        }

        private bool TryGetModelValue(string prop, out object value)
        {
            value = null;
            var model = GetModel();
            if (model == null || string.IsNullOrWhiteSpace(prop))
            {
                return false;
            }

            return FormModelPath.TryGetValue(model, prop, out value);
        }

        internal IEnumerable<IValidationRule> GetDataAnnotationRules(ElFormItemObject item)
        {
            var model = GetModel();
            if (model == null || item == null || string.IsNullOrWhiteSpace(item.Name))
            {
                return Enumerable.Empty<IValidationRule>();
            }

            if (!FormModelPath.TryGetPropertyInfo(model.GetType(), item.Name, out var property))
            {
                return Enumerable.Empty<IValidationRule>();
            }

            var attributes = property.GetCustomAttributes<ValidationAttribute>(inherit: true).ToArray();
            if (attributes.Length == 0)
            {
                return Enumerable.Empty<IValidationRule>();
            }

            return attributes.Select(attribute => new ValidationAttributeRule(attribute, () =>
            {
                FormModelPath.TryGetParentObject(model, item.Name, out var parent);
                var context = new ValidationContext(parent ?? model)
                {
                    MemberName = property.Name,
                    DisplayName = ResolveDisplayName(property)
                };
                return context;
            })).ToArray();
        }

        private static string ResolveDisplayName(PropertyInfo property)
        {
            var display = property.GetCustomAttribute<DisplayAttribute>();
            if (display != null)
            {
                return display.GetName() ?? display.Name ?? property.Name;
            }

            var displayName = property.GetCustomAttribute<System.ComponentModel.DisplayNameAttribute>();
            return displayName?.DisplayName ?? property.Name;
        }

        private IEnumerable<ElFormItemObject> FilterItems(params string[] props)
        {
            if (props == null || props.Length == 0 || props.All(string.IsNullOrWhiteSpace))
            {
                return Items;
            }

            var propSet = new HashSet<string>(props.Where(x => !string.IsNullOrWhiteSpace(x)), StringComparer.OrdinalIgnoreCase);
            return Items.Where(x => propSet.Contains(x.Name) || propSet.Contains(x.Prop));
        }

        private void RefreshFieldInitialValues(bool resetCurrentValue)
        {
            foreach (var item in Items)
            {
                ApplyInitialValue(item, resetCurrentValue);
            }
        }

        private void ApplyInitialValue(ElFormItemObject item, bool resetCurrentValue)
        {
            if (item == null || !Values.Any() || string.IsNullOrWhiteSpace(item.Name))
            {
                return;
            }

            if (Values.TryGetValue(item.Name, out var value) || TryGetModelValue(item.Name, out value))
            {
                item.SetInitialValue(value, resetCurrentValue);
            }
        }

        private static object ConvertValue(object value, Type destinationType)
        {
            if (destinationType == null)
            {
                return value;
            }

            if (value == null)
            {
                return null;
            }

            var finalType = Nullable.GetUnderlyingType(destinationType) ?? destinationType;
            if (finalType.IsInstanceOfType(value))
            {
                return value;
            }

            if (finalType.IsEnum)
            {
                if (value is string stringValue)
                {
                    return Enum.Parse(finalType, stringValue);
                }
                return Enum.ToObject(finalType, value);
            }

            if (destinationType == typeof(string[]))
            {
                if (value is IEnumerable<string> stringEnumerable)
                {
                    return stringEnumerable.ToArray();
                }
            }

            if (destinationType == typeof(List<string>))
            {
                if (value is IEnumerable<string> stringEnumerable)
                {
                    return stringEnumerable.ToList();
                }
            }

            if (destinationType == typeof(IList<string>))
            {
                if (value is IEnumerable<string> stringEnumerable)
                {
                    return stringEnumerable.ToList();
                }
            }

            if (destinationType == typeof(decimal?) && value is double doubleValue)
            {
                return (decimal)doubleValue;
            }

            if (destinationType == typeof(double?) && value is decimal decimalValue)
            {
                return (double)decimalValue;
            }

            return TypeHelper.ChangeType(value, destinationType);
        }

        private static string NormalizeLabelPosition(LabelAlign labelAlign) => labelAlign switch
        {
            LabelAlign.Left => "left",
            LabelAlign.Top => "top",
            _ => "right"
        };

        private static string GetSizeCssValue(InputSize size) => size switch
        {
            InputSize.Large => "large",
            InputSize.Small => "small",
            _ => null
        };

        private readonly struct FormInputRegistration
        {
            public FormInputRegistration(string prop, string id, object input, ElFormItemObject field)
            {
                Prop = prop;
                Id = id;
                Input = input;
                Field = field;
            }

            public string Prop { get; }

            public string Id { get; }

            public object Input { get; }

            public ElFormItemObject Field { get; }
        }
    }

    public class FormValidateEventArgs
    {
        public string Prop { get; set; }

        public bool IsValid { get; set; }

        public string Message { get; set; }
    }

    public class ElFormSubmitEventArgs
    {
        public ElFormSubmitEventArgs(ElForm form)
        {
            Form = form;
        }

        public ElForm Form { get; }

        public bool IsValid { get; set; }
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

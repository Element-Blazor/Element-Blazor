using Element.ControlConfigs;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Linq;
using System.Reflection;

namespace Element.ControlRenders
{
    internal class FormControlRender : RenderBase, Element.ControlRender.IControlRender
    {
        public void Render(RenderTreeBuilder builder, RenderConfig config)
        {
            var controlType = config.InputControlType;
            var genericDefinition = controlType.IsGenericType ? controlType.GetGenericTypeDefinition() : null;
            var seq = 0;
            builder.OpenComponent(seq++, controlType);
            builder.AddAttribute(seq++, nameof(ElFormItemObject.EnableAlwaysRender), true);
            seq = AddCommonAttributes(builder, seq, config.ControlAttribute as BaseAttribute);

            if (controlType == typeof(ElInputNumber))
            {
                RenderInputNumber(builder, config, ref seq);
            }
            else if (controlType == typeof(ElInputTag))
            {
                RenderInputTag(builder, config, ref seq);
            }
            else if (controlType == typeof(ElInputOtp))
            {
                RenderInputOtp(builder, config, ref seq);
            }
            else if (controlType == typeof(ElMention))
            {
                RenderMention(builder, config, ref seq);
            }
            else if (controlType == typeof(ElRate))
            {
                RenderRate(builder, config, ref seq);
            }
            else if (controlType == typeof(ElSlider))
            {
                RenderSlider(builder, config, ref seq);
            }
            else if (controlType == typeof(ElTimePicker))
            {
                RenderTimePicker(builder, config, ref seq);
            }
            else if (genericDefinition == typeof(ElRadioGroup<>))
            {
                RenderRadioGroup(builder, config, ref seq);
            }
            else if (genericDefinition == typeof(ElSelectV2<>))
            {
                RenderSelectV2(builder, config, ref seq);
            }
            else
            {
                throw new ElementException($"组件 {controlType.FullName} 尚未实现对应的表单渲染器");
            }

            builder.CloseComponent();
        }

        private static int AddCommonAttributes(RenderTreeBuilder builder, int seq, BaseAttribute attribute)
        {
            if (attribute == null)
            {
                return seq;
            }
            if (!string.IsNullOrWhiteSpace(attribute.Style))
            {
                builder.AddAttribute(seq++, nameof(ElementComponentBase.Style), attribute.Style);
            }
            return seq;
        }

        private void RenderInputNumber(RenderTreeBuilder builder, RenderConfig config, ref int seq)
        {
            var attribute = (InputNumberAttribute)config.ControlAttribute;
            if (attribute != null)
            {
                if (!double.IsNaN(attribute.Min))
                {
                    builder.AddAttribute(seq++, nameof(ElInputNumber.Min), (decimal)attribute.Min);
                }
                if (!double.IsNaN(attribute.Max))
                {
                    builder.AddAttribute(seq++, nameof(ElInputNumber.Max), (decimal)attribute.Max);
                }
                builder.AddAttribute(seq++, nameof(ElInputNumber.Step), (decimal)attribute.Step);
                builder.AddAttribute(seq++, nameof(ElInputNumber.StepStrictly), attribute.StepStrictly);
                if (attribute.Precision >= 0)
                {
                    builder.AddAttribute(seq++, nameof(ElInputNumber.Precision), attribute.Precision);
                }
                builder.AddAttribute(seq++, nameof(ElInputNumber.Controls), attribute.Controls);
                builder.AddAttribute(seq++, nameof(ElInputNumber.ControlsPosition), attribute.ControlsPosition);
                builder.AddAttribute(seq++, nameof(ElInputNumber.IsDisabled), attribute.IsDisabled);
                builder.AddAttribute(seq++, nameof(ElInputNumber.Readonly), attribute.Readonly);
                builder.AddAttribute(seq++, nameof(ElInputNumber.Size), attribute.Size);
                builder.AddAttribute(seq++, nameof(ElInputNumber.Autocomplete), attribute.Autocomplete);
                builder.AddAttribute(seq++, nameof(ElInputNumber.Inputmode), attribute.Inputmode);
                builder.AddAttribute(seq++, nameof(ElInputNumber.Tabindex), attribute.Tabindex);
                builder.AddAttribute(seq++, nameof(ElInputNumber.ValidateEvent), attribute.ValidateEvent);
                builder.AddAttribute(seq++, nameof(ElInputNumber.Placeholder), attribute.Placeholder ?? config.Placeholder);
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInputNumber.AriaLabel), attribute.AriaLabel);
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInputNumber.AriaLabelledby), attribute.AriaLabelledby);
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInputNumber.AriaDescribedby), attribute.AriaDescribedby);
            }
            else if (!string.IsNullOrWhiteSpace(config.Placeholder))
            {
                builder.AddAttribute(seq++, nameof(ElInputNumber.Placeholder), config.Placeholder);
            }
            CreateBind(config, builder, seq, nameof(ElInputNumber.ValueChanged), nameof(ElInputNumber.Value), typeof(decimal?));
            seq += 2;
        }

        private void RenderInputTag(RenderTreeBuilder builder, RenderConfig config, ref int seq)
        {
            var attribute = (InputTagAttribute)config.ControlAttribute;
            if (attribute != null)
            {
                builder.AddAttribute(seq++, nameof(ElInputTag.Placeholder), attribute.Placeholder ?? config.Placeholder);
                builder.AddAttribute(seq++, nameof(ElInputTag.Clearable), attribute.Clearable);
                builder.AddAttribute(seq++, nameof(ElInputTag.IsDisabled), attribute.IsDisabled);
                builder.AddAttribute(seq++, nameof(ElInputTag.Readonly), attribute.Readonly);
                builder.AddAttribute(seq++, nameof(ElInputTag.Size), attribute.Size);
                if (attribute.Max > 0)
                {
                    builder.AddAttribute(seq++, nameof(ElInputTag.Max), attribute.Max);
                }
                builder.AddAttribute(seq++, nameof(ElInputTag.AllowDuplicates), attribute.AllowDuplicates);
                builder.AddAttribute(seq++, nameof(ElInputTag.TriggerKeys), attribute.TriggerKeys);
                builder.AddAttribute(seq++, nameof(ElInputTag.AddOnBlur), attribute.AddOnBlur);
                builder.AddAttribute(seq++, nameof(ElInputTag.Draggable), attribute.Draggable);
            }
            else if (!string.IsNullOrWhiteSpace(config.Placeholder))
            {
                builder.AddAttribute(seq++, nameof(ElInputTag.Placeholder), config.Placeholder);
            }
            CreateBind(config, builder, seq, nameof(ElInputTag.ValueChanged), nameof(ElInputTag.Value), typeof(IList<string>));
            seq += 2;
        }

        private static void AddAttributeIfNotEmpty(RenderTreeBuilder builder, ref int seq, string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }

            builder.AddAttribute(seq++, name, value);
        }

        private void RenderInputOtp(RenderTreeBuilder builder, RenderConfig config, ref int seq)
        {
            var attribute = (InputOtpAttribute)config.ControlAttribute;
            if (attribute != null)
            {
                builder.AddAttribute(seq++, nameof(ElInputOtp.Length), attribute.Length);
                builder.AddAttribute(seq++, nameof(ElInputOtp.IsDisabled), attribute.IsDisabled);
                builder.AddAttribute(seq++, nameof(ElInputOtp.Readonly), attribute.Readonly);
                builder.AddAttribute(seq++, nameof(ElInputOtp.Inputmode), attribute.Inputmode);
                builder.AddAttribute(seq++, nameof(ElInputOtp.Mask), attribute.Mask);
            }
            CreateBind(config, builder, seq, nameof(ElInputOtp.ValueChanged), nameof(ElInputOtp.Value), typeof(string));
            seq += 2;
        }

        private void RenderMention(RenderTreeBuilder builder, RenderConfig config, ref int seq)
        {
            var attribute = (MentionAttribute)config.ControlAttribute;
            if (attribute != null)
            {
                builder.AddAttribute(seq++, nameof(ElMention.Placeholder), attribute.Placeholder ?? config.Placeholder);
                builder.AddAttribute(seq++, nameof(ElMention.Prefix), attribute.Prefix);
                if (attribute.Prefixes != null)
                {
                    builder.AddAttribute(seq++, nameof(ElMention.Prefixes), attribute.Prefixes);
                }
                builder.AddAttribute(seq++, nameof(ElMention.Rows), attribute.Rows);
                builder.AddAttribute(seq++, nameof(ElMention.Autocomplete), attribute.Autocomplete);
                builder.AddAttribute(seq++, nameof(ElMention.Tabindex), attribute.Tabindex);
                builder.AddAttribute(seq++, nameof(ElMention.IsDisabled), attribute.IsDisabled);
                builder.AddAttribute(seq++, nameof(ElMention.Readonly), attribute.Readonly);
                builder.AddAttribute(seq++, nameof(ElMention.Size), attribute.Size);
                builder.AddAttribute(seq++, nameof(ElMention.Clearable), attribute.Clearable);
                builder.AddAttribute(seq++, nameof(ElMention.Options), CreateMentionOptions(config.DataSource, attribute));
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElMention.AriaLabel), attribute.AriaLabel);
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElMention.InputStyle), attribute.InputStyle);
            }
            else if (!string.IsNullOrWhiteSpace(config.Placeholder))
            {
                builder.AddAttribute(seq++, nameof(ElMention.Placeholder), config.Placeholder);
            }
            CreateBind(config, builder, seq, nameof(ElMention.ValueChanged), nameof(ElMention.Value), typeof(string));
            seq += 2;
        }

        private void RenderRate(RenderTreeBuilder builder, RenderConfig config, ref int seq)
        {
            var attribute = (RateAttribute)config.ControlAttribute;
            if (attribute != null)
            {
                builder.AddAttribute(seq++, nameof(ElRate.Max), attribute.Max);
                builder.AddAttribute(seq++, nameof(ElRate.IsDisabled), attribute.IsDisabled);
                builder.AddAttribute(seq++, nameof(ElRate.AllowHalf), attribute.AllowHalf);
                builder.AddAttribute(seq++, nameof(ElRate.Clearable), attribute.Clearable);
                builder.AddAttribute(seq++, nameof(ElRate.ShowText), attribute.ShowText);
                builder.AddAttribute(seq++, nameof(ElRate.ShowScore), attribute.ShowScore);
                builder.AddAttribute(seq++, nameof(ElRate.ScoreTemplate), attribute.ScoreTemplate);
                if (attribute.Texts != null)
                {
                    builder.AddAttribute(seq++, nameof(ElRate.Texts), attribute.Texts);
                }
                if (attribute.Colors != null)
                {
                    builder.AddAttribute(seq++, nameof(ElRate.Colors), attribute.Colors);
                }
                if (!string.IsNullOrWhiteSpace(attribute.VoidColor))
                {
                    builder.AddAttribute(seq++, nameof(ElRate.VoidColor), attribute.VoidColor);
                }
                if (!string.IsNullOrWhiteSpace(attribute.DisabledVoidColor))
                {
                    builder.AddAttribute(seq++, nameof(ElRate.DisabledVoidColor), attribute.DisabledVoidColor);
                }
                if (!string.IsNullOrWhiteSpace(attribute.Icon))
                {
                    builder.AddAttribute(seq++, nameof(ElRate.Icon), attribute.Icon);
                }
                if (!string.IsNullOrWhiteSpace(attribute.VoidIcon))
                {
                    builder.AddAttribute(seq++, nameof(ElRate.VoidIcon), attribute.VoidIcon);
                }
            }
            CreateBind(config, builder, seq, nameof(ElRate.ValueChanged), nameof(ElRate.Value), typeof(double?));
            seq += 2;
        }

        private void RenderSlider(RenderTreeBuilder builder, RenderConfig config, ref int seq)
        {
            var attribute = (SliderAttribute)config.ControlAttribute;
            if (attribute != null)
            {
                builder.AddAttribute(seq++, nameof(ElSlider.Min), attribute.Min);
                builder.AddAttribute(seq++, nameof(ElSlider.Max), attribute.Max);
                builder.AddAttribute(seq++, nameof(ElSlider.Step), attribute.Step);
                builder.AddAttribute(seq++, nameof(ElSlider.IsDisabled), attribute.IsDisabled);
                builder.AddAttribute(seq++, nameof(ElSlider.ShowStops), attribute.ShowStops);
                builder.AddAttribute(seq++, nameof(ElSlider.ShowInput), attribute.ShowInput);
                if (attribute.Marks != null)
                {
                    builder.AddAttribute(seq++, nameof(ElSlider.Marks), attribute.Marks);
                }
            }
            CreateBind(config, builder, seq, nameof(ElSlider.ValueChanged), nameof(ElSlider.Value), typeof(double));
            seq += 2;
        }

        private void RenderTimePicker(RenderTreeBuilder builder, RenderConfig config, ref int seq)
        {
            var attribute = (TimePickerAttribute)config.ControlAttribute;
            if (attribute != null)
            {
                builder.AddAttribute(seq++, nameof(ElTimePicker.Type), attribute.IsRange ? TimePickerType.TimeRange : attribute.Type);
                builder.AddAttribute(seq++, nameof(ElTimePicker.IsRange), attribute.IsRange);
                builder.AddAttribute(seq++, nameof(ElTimePicker.Format), attribute.Format);
                if (!string.IsNullOrWhiteSpace(attribute.ValueFormat))
                {
                    builder.AddAttribute(seq++, nameof(ElTimePicker.ValueFormat), attribute.ValueFormat);
                }
                builder.AddAttribute(seq++, nameof(ElTimePicker.Placeholder), attribute.Placeholder ?? config.Placeholder);
                builder.AddAttribute(seq++, nameof(ElTimePicker.StartPlaceholder), attribute.StartPlaceholder);
                builder.AddAttribute(seq++, nameof(ElTimePicker.EndPlaceholder), attribute.EndPlaceholder);
                builder.AddAttribute(seq++, nameof(ElTimePicker.RangeSeparator), attribute.RangeSeparator);
                builder.AddAttribute(seq++, nameof(ElTimePicker.IsDisabled), attribute.IsDisabled);
                builder.AddAttribute(seq++, nameof(ElTimePicker.Readonly), attribute.Readonly);
                builder.AddAttribute(seq++, nameof(ElTimePicker.Editable), attribute.Editable);
                builder.AddAttribute(seq++, nameof(ElTimePicker.Clearable), attribute.Clearable);
                builder.AddAttribute(seq++, nameof(ElTimePicker.Size), attribute.Size);
                builder.AddAttribute(seq++, nameof(ElTimePicker.PrefixIcon), attribute.PrefixIcon);
                builder.AddAttribute(seq++, nameof(ElTimePicker.ClearIcon), attribute.ClearIcon);
                builder.AddAttribute(seq++, nameof(ElTimePicker.ValidateEvent), attribute.ValidateEvent);
            }
            else if (!string.IsNullOrWhiteSpace(config.Placeholder))
            {
                builder.AddAttribute(seq++, nameof(ElTimePicker.Placeholder), config.Placeholder);
            }
            CreateTimePickerBind(config, builder, seq);
            seq += 2;
        }

        private void CreateTimePickerBind(RenderConfig config, RenderTreeBuilder builder, int startIndex)
        {
            CreateTwoWayBinding(config, builder, startIndex, nameof(ElTimePicker.ValueChanged), typeof(TimeSpan?));
            var value = config.EditingValue ?? config.RawValue;
            if (value == null)
            {
                return;
            }

            builder.AddAttribute(startIndex + 1, nameof(ElTimePicker.Value), ConvertToTimeSpan(value));
        }

        private void RenderRadioGroup(RenderTreeBuilder builder, RenderConfig config, ref int seq)
        {
            var attribute = (RadioAttribute)config.ControlAttribute;
            var valueType = config.ValueType ?? config.InputControlType.GetGenericArguments()[0];
            if (attribute != null)
            {
                builder.AddAttribute(seq++, nameof(ElRadioGroup<string>.Size), attribute.Size);
                builder.AddAttribute(seq++, nameof(ElRadioGroup<string>.IsDisabled), attribute.IsDisabled);
                builder.AddAttribute(seq++, nameof(ElRadioGroup<string>.Bordered), attribute.Bordered);
            }

            builder.AddAttribute(seq++, nameof(ElRadioGroup<string>.ChildContent), (RenderFragment)(contentBuilder =>
            {
                var childSeq = 0;
                foreach (var option in CreateOptions(config, attribute?.Display, attribute?.Value, valueType))
                {
                    var radioType = (attribute?.Button ?? false)
                        ? typeof(ElRadioButton<>).MakeGenericType(valueType)
                        : typeof(ElRadio<>).MakeGenericType(valueType);
                    contentBuilder.OpenComponent(childSeq++, radioType);
                    contentBuilder.AddAttribute(childSeq++, nameof(ElRadio<string>.Value), option.Value);
                    contentBuilder.AddAttribute(childSeq++, nameof(ElRadio<string>.IsDisabled), option.Disabled);
                    contentBuilder.AddAttribute(childSeq++, nameof(ElRadio<string>.ChildContent), Text(option.Text));
                    contentBuilder.CloseComponent();
                }
            }));

            CreateBind(config, builder, seq, nameof(ElRadioGroup<string>.SelectedValueChanged), nameof(ElRadioGroup<string>.SelectedValue), valueType);
            seq += 2;
        }

        private void RenderSelectV2(RenderTreeBuilder builder, RenderConfig config, ref int seq)
        {
            var attribute = (SelectAttribute)config.ControlAttribute;
            var valueType = config.ValueType ?? config.InputControlType.GetGenericArguments()[0];
            if (attribute != null)
            {
                AddSelectAttributes(builder, attribute, ref seq);
                builder.AddAttribute(seq++, nameof(ElSelectV2<string>.ItemHeight), attribute.ItemHeight);
                builder.AddAttribute(seq++, nameof(ElSelectV2<string>.Height), attribute.Height);
            }
            if (!string.IsNullOrWhiteSpace(config.Placeholder))
            {
                builder.AddAttribute(seq++, nameof(ElSelectV2<string>.Placeholder), config.Placeholder);
            }
            builder.AddAttribute(seq++, nameof(ElSelectV2<string>.Options), CreateSelectV2Options(config, attribute?.Display, attribute?.Value, valueType));
            CreateBind(config, builder, seq, nameof(ElSelectV2<string>.ValueChanged), nameof(ElSelectV2<string>.Value), valueType);
            seq += 2;
        }

        internal static void AddSelectAttributes(RenderTreeBuilder builder, SelectAttribute attribute, ref int seq)
        {
            if (attribute == null)
            {
                return;
            }
            builder.AddAttribute(seq++, nameof(ElSelect<string>.Placeholder), attribute.Placeholder);
            builder.AddAttribute(seq++, nameof(ElSelect<string>.IsDisabled), attribute.IsDisabled);
            builder.AddAttribute(seq++, nameof(ElSelect<string>.Clearable), attribute.Clearable);
            builder.AddAttribute(seq++, nameof(ElSelect<string>.Filterable), attribute.Filterable);
            builder.AddAttribute(seq++, nameof(ElSelect<string>.Multiple), attribute.Multiple);
            builder.AddAttribute(seq++, nameof(ElSelect<string>.CollapseTags), attribute.CollapseTags);
            builder.AddAttribute(seq++, nameof(ElSelect<string>.MaxCollapseTags), attribute.MaxCollapseTags);
        }

        internal static RenderFragment CreateSelectContent(RenderConfig config, string displayProperty, string valueProperty, Type valueType)
        {
            return contentBuilder =>
            {
                var seq = 0;
                foreach (var option in CreateOptions(config, displayProperty, valueProperty, valueType))
                {
                    contentBuilder.OpenComponent(seq++, typeof(ElOption<>).MakeGenericType(valueType));
                    contentBuilder.AddAttribute(seq++, nameof(ElOption<string>.Value), option.Value);
                    contentBuilder.AddAttribute(seq++, nameof(ElOption<string>.Text), option.Text);
                    contentBuilder.AddAttribute(seq++, nameof(ElOption<string>.Disabled), option.Disabled);
                    contentBuilder.CloseComponent();
                }
            };
        }

        internal static IEnumerable<SelectV2Option> CreateSelectV2Options(RenderConfig config, string displayProperty, string valueProperty, Type valueType)
        {
            return CreateOptions(config, displayProperty, valueProperty, valueType)
                .Select(x => new SelectV2Option
                {
                    Value = x.Value,
                    Label = x.Text,
                    Disabled = x.Disabled
                })
                .ToList();
        }

        private static IEnumerable<MentionOption> CreateMentionOptions(object dataSource, MentionAttribute attribute)
        {
            if (dataSource == null)
            {
                return Enumerable.Empty<MentionOption>();
            }

            return AsEnumerable(dataSource).Select(item =>
            {
                var value = GetPropertyValue(item, attribute?.Value) ?? item;
                var text = GetPropertyValue(item, attribute?.Display) ?? value;
                return new MentionOption
                {
                    Value = Convert.ToString(value),
                    Label = Convert.ToString(text),
                    Disabled = Convert.ToBoolean(GetPropertyValue(item, "Disabled") ?? false)
                };
            }).ToList();
        }

        private static IEnumerable<OptionItem> CreateOptions(RenderConfig config, string displayProperty, string valueProperty, Type valueType)
        {
            var finalValueType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            if (config.DataSource != null)
            {
                foreach (var item in AsEnumerable(config.DataSource))
                {
                    var rawValue = GetPropertyValue(item, valueProperty) ?? item;
                    var rawText = GetPropertyValue(item, displayProperty)
                        ?? GetPropertyValue(item, "Text")
                        ?? GetPropertyValue(item, "Label")
                        ?? rawValue;
                    yield return new OptionItem
                    {
                        Value = ConvertValue(rawValue, valueType),
                        Text = Convert.ToString(rawText),
                        Disabled = Convert.ToBoolean(GetPropertyValue(item, "Disabled") ?? false)
                    };
                }
                yield break;
            }

            if (!finalValueType.IsEnum)
            {
                yield break;
            }

            foreach (var name in Enum.GetNames(finalValueType))
            {
                var field = finalValueType.GetField(name);
                var description = field.GetCustomAttribute<DescriptionAttribute>()?.Description;
                var display = field.GetCustomAttribute<DisplayAttribute>();
                yield return new OptionItem
                {
                    Value = ConvertValue(Enum.Parse(finalValueType, name), valueType),
                    Text = description ?? display?.Name ?? display?.Description ?? name
                };
            }
        }

        private static IEnumerable<object> AsEnumerable(object dataSource)
        {
            return dataSource is IEnumerable enumerable && dataSource is not string
                ? enumerable.Cast<object>()
                : Enumerable.Empty<object>();
        }

        private static object GetPropertyValue(object item, string propertyName)
        {
            if (item == null || string.IsNullOrWhiteSpace(propertyName))
            {
                return null;
            }
            return item.GetType().GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)?.GetValue(item);
        }

        private static object ConvertValue(object value, Type valueType)
        {
            if (value == null)
            {
                return null;
            }
            var finalType = Nullable.GetUnderlyingType(valueType) ?? valueType;
            if (finalType.IsInstanceOfType(value))
            {
                return value;
            }
            if (finalType.IsEnum)
            {
                return value is string stringValue
                    ? Enum.Parse(finalType, stringValue)
                    : Enum.ToObject(finalType, value);
            }
            return TypeHelper.ChangeType(value, valueType);
        }

        private static TimeSpan? ConvertToTimeSpan(object value)
        {
            if (value == null)
            {
                return null;
            }
            if (value is TimeSpan time)
            {
                return time;
            }
            if (value is DateTime dateTime)
            {
                return dateTime.TimeOfDay;
            }
            if (TimeSpan.TryParse(Convert.ToString(value, CultureInfo.CurrentCulture), CultureInfo.CurrentCulture, out var current))
            {
                return current;
            }
            if (TimeSpan.TryParse(Convert.ToString(value, CultureInfo.InvariantCulture), CultureInfo.InvariantCulture, out var invariant))
            {
                return invariant;
            }
            return null;
        }

        private static RenderFragment Text(string text)
        {
            return builder => builder.AddContent(0, text);
        }

        private sealed class OptionItem
        {
            public object Value { get; set; }

            public string Text { get; set; }

            public bool Disabled { get; set; }
        }
    }
}

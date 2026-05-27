using Element.ControlConfigs;
using Element.ControlRender;
using Microsoft.AspNetCore.Components.Rendering;

namespace Element.ControlRenders
{
    internal class InputRender : RenderBase, IInputRender
    {
        public void Render(RenderTreeBuilder builder, RenderConfig config)
        {
            var inputConfig = (InputAttribute)config.ControlAttribute;
            var seq = 0;
            builder.OpenComponent(seq++, config.InputControlType);
            builder.AddAttribute(seq++, nameof(ElFormItemObject.EnableAlwaysRender), true);

            if (inputConfig != null)
            {
                builder.AddAttribute(seq++, nameof(ElInput<string>.Clearable), inputConfig.Clearable);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Disabled), inputConfig.Disabled);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Readonly), inputConfig.Readonly);
                builder.AddAttribute(seq++, nameof(ElInput<string>.PrefixIcon), inputConfig.PrefixIcon);
                builder.AddAttribute(seq++, nameof(ElInput<string>.SuffixIcon), inputConfig.SuffixIcon);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Type), inputConfig.Type);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Image), inputConfig.Image);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Style), inputConfig.Style);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Size), inputConfig.Size);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Autosize), inputConfig.Autosize);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Autocomplete), inputConfig.Autocomplete);
                builder.AddAttribute(seq++, nameof(ElInput<string>.ShowPassword), inputConfig.ShowPassword);
                builder.AddAttribute(seq++, nameof(ElInput<string>.ShowWordLimit), inputConfig.ShowWordLimit);
                builder.AddAttribute(seq++, nameof(ElInput<string>.WordLimitPosition), inputConfig.WordLimitPosition);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Autofocus), inputConfig.Autofocus);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Rows), inputConfig.Rows);
                builder.AddAttribute(seq++, nameof(ElInput<string>.Tabindex), inputConfig.Tabindex);
                builder.AddAttribute(seq++, nameof(ElInput<string>.ValidateEvent), inputConfig.ValidateEvent);

                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInput<string>.Placeholder), inputConfig.Placeholder ?? config.Placeholder);
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInput<string>.Resize), inputConfig.Resize);
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInput<string>.Form), inputConfig.Form);
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInput<string>.InputStyle), inputConfig.InputStyle);
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInput<string>.AriaLabel), inputConfig.AriaLabel);
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInput<string>.Inputmode), inputConfig.Inputmode);
                AddAttributeIfNonNegative(builder, ref seq, nameof(ElInput<string>.Maxlength), inputConfig.Maxlength);
                AddAttributeIfNonNegative(builder, ref seq, nameof(ElInput<string>.Minlength), inputConfig.Minlength);
            }
            else
            {
                AddAttributeIfNotEmpty(builder, ref seq, nameof(ElInput<string>.Placeholder), config.Placeholder);
            }

            CreateBind(config, builder, seq);
            builder.CloseComponent();
        }

        private static void AddAttributeIfNotEmpty(RenderTreeBuilder builder, ref int seq, string name, string value)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return;
            }
            builder.AddAttribute(seq++, name, value);
        }

        private static void AddAttributeIfNonNegative(RenderTreeBuilder builder, ref int seq, string name, int value)
        {
            if (value < 0)
            {
                return;
            }
            builder.AddAttribute(seq++, name, value);
        }
    }
}

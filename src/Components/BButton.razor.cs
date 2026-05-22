using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BButton : BComponentBase
    {
        internal HtmlPropertyBuilder cssClassBuilder;
        internal HtmlPropertyBuilder buttonStyleBuilder;

        [CascadingParameter]
        public BButtonGroup ButtonGroup { get; set; }

        protected async Task OnButtonClickedAsync(MouseEventArgs e)
        {
            if (IsButtonDisabled)
            {
                return;
            }
            var oldImg = showingImage;
            if (!string.IsNullOrWhiteSpace(showingImage) && !string.IsNullOrWhiteSpace(ClickImage))
            {
                showingImage = ClickImage;
                StateHasChanged();
                await Task.Delay(100);
            }
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }
            if (!string.IsNullOrWhiteSpace(oldImg))
            {
                showingImage = oldImg;
                StateHasChanged();
            }
        }

        private string showingImage;

        /// <summary>
        /// 文本按钮外观。Element Plus 对齐参数。
        /// </summary>
        [Parameter]
        public bool Text { get; set; }

        /// <summary>
        /// 文本内容。Razor 推荐直接使用 ChildContent。
        /// </summary>
        [Parameter]
        public string Content { get; set; }

        /// <summary>
        /// 文本内容别名。
        /// </summary>
        [Parameter]
        public string Label { get; set; }

        /// <summary>
        /// 按钮图片
        /// </summary>
        [Parameter]
        public string Image { get; set; }

        /// <summary>
        /// 鼠标放上去时的按钮图片
        /// </summary>
        [Parameter]
        public string HoverImage { get; set; }

        /// <summary>
        /// 鼠标点击时的按钮图片
        /// </summary>
        [Parameter]
        public string ClickImage { get; set; }

        /// <summary>
        /// 是否将自定义的 CSS 类加入到已有 CSS 类，如果为 false，则替换掉默认 CSS 类，默认为 true
        /// </summary>
        [Parameter]
        public bool AppendCustomCls { get; set; } = true;
        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public ButtonType Type { get; set; } = ButtonType.Default;

        [Parameter]
        public bool Plain { get; set; }

        [Parameter]
        public bool IsPlain
        {
            get => Plain;
            set => Plain = value;
        }

        [Parameter]
        public bool Round { get; set; }

        [Parameter]
        public bool IsRound
        {
            get => Round;
            set => Round = value;
        }

        [Parameter]
        public bool Disabled { get; set; }

        [Parameter]
        public bool IsDisabled
        {
            get => Disabled;
            set => Disabled = value;
        }

        [Parameter]
        public bool Circle { get; set; }

        [Parameter]
        public bool IsCircle
        {
            get => Circle;
            set => Circle = value;
        }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public ButtonSize Size { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public RenderFragment IconContent { get; set; }

        [Parameter]
        public bool Loading { get; set; }

        [Parameter]
        public bool IsLoading
        {
            get => Loading;
            set => Loading = value;
        }

        [Parameter]
        public string LoadingIcon { get; set; } = "el-icon-loading";

        [Parameter]
        public RenderFragment LoadingContent { get; set; }

        [Parameter]
        public string NativeType { get; set; } = "button";

        [Parameter]
        public bool Link { get; set; }

        [Parameter]
        public bool Bg { get; set; }

        [Parameter]
        public bool Dashed { get; set; }

        [Parameter]
        public bool Autofocus { get; set; }

        [Parameter]
        public string Color { get; set; }

        [Parameter]
        public bool Dark { get; set; }

        [Parameter]
        public bool AutoInsertSpace { get; set; }

        [Parameter]
        public string Tag { get; set; } = "button";

        [Parameter]
        public string Href { get; set; }

        [Parameter]
        public string Target { get; set; }

        [Parameter]
        public string Rel { get; set; }

        [Parameter]
        public int? Tabindex { get; set; }

        protected string ButtonTextContent => Content ?? Label;

        protected bool IsButtonDisabled => Disabled || Loading;

        private ButtonType EffectiveType => Type == ButtonType.Default && ButtonGroup != null
            ? ButtonGroup.Type
            : Type;

        private ButtonSize EffectiveSize => Size == ButtonSize.Default && ButtonGroup != null
            ? ButtonGroup.Size
            : Size;

        private bool IsTextButton => Text || EffectiveType == ButtonType.Text;

        private string EffectiveTag => string.IsNullOrWhiteSpace(Tag) ? "button" : Tag.Trim().ToLowerInvariant();

        private bool IsNativeButton => string.Equals(EffectiveTag, "button", StringComparison.OrdinalIgnoreCase);

        private string SizeCssValue => EffectiveSize switch
        {
            ButtonSize.Large => "large",
            ButtonSize.Small => "small",
            _ => null
        };

        private string TypeCssValue => EffectiveType == ButtonType.Default || EffectiveType == ButtonType.Text
            ? null
            : EffectiveType.ToString().ToLowerInvariant();

        private string RenderedButtonTextContent
        {
            get
            {
                var text = ButtonTextContent ?? string.Empty;
                if (!AutoInsertSpace || text.Length != 2 || !text.All(IsChineseCharacter))
                {
                    return text;
                }

                return $"{text[0]} {text[1]}";
            }
        }

        private static bool IsChineseCharacter(char value) => value >= '\u4e00' && value <= '\u9fff';

        protected override bool ShouldRender()
        {
            return true;
        }

        private void MouseOver()
        {
            if (string.IsNullOrWhiteSpace(HoverImage))
            {
                return;
            }
            showingImage = HoverImage;
        }
        private void MouseOut()
        {
            if (string.IsNullOrWhiteSpace(HoverImage))
            {
                return;
            }

            showingImage = Image;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (string.IsNullOrWhiteSpace(showingImage) && !string.IsNullOrWhiteSpace(Image))
            {
                showingImage = Image;
            }
            cssClassBuilder = HtmlPropertyBuilder.CreateCssClassBuilder();
            buttonStyleBuilder = HtmlPropertyBuilder.CreateCssStyleBuilder()
                .Add(Style)
                .AddIf(!string.IsNullOrWhiteSpace(Color) && !IsTextButton && !Link,
                    $"--el-button-text-color:{(Dark ? "var(--el-color-white)" : "var(--el-color-white)")}",
                    $"--el-button-bg-color:{Color}",
                    $"--el-button-border-color:{Color}",
                    $"--el-button-hover-bg-color:color-mix(in srgb, {Color} 82%, var(--el-color-white))",
                    $"--el-button-hover-border-color:color-mix(in srgb, {Color} 82%, var(--el-color-white))",
                    $"--el-button-active-bg-color:color-mix(in srgb, {Color} 82%, var(--el-color-black))",
                    $"--el-button-active-border-color:color-mix(in srgb, {Color} 82%, var(--el-color-black))")
                .AddIf(!string.IsNullOrWhiteSpace(Color) && (IsTextButton || Link),
                    $"--el-button-text-color:{Color}",
                    $"--el-button-hover-text-color:color-mix(in srgb, {Color} 82%, var(--el-color-white))",
                    $"--el-button-active-text-color:color-mix(in srgb, {Color} 82%, var(--el-color-black))");
            if (string.IsNullOrWhiteSpace(Cls) || AppendCustomCls)
            {
                cssClassBuilder.Add("el-button", Cls)
                .AddIf(TypeCssValue != null, $"el-button--{TypeCssValue}")
                .AddIf(SizeCssValue != null, $"el-button--{SizeCssValue}")
                .AddIf(Plain, "is-plain")
                .AddIf(Round, "is-round")
                .AddIf(IsButtonDisabled, "is-disabled")
                .AddIf(Loading, "is-loading")
                .AddIf(Circle, "is-circle")
                .AddIf(IsTextButton, "is-text")
                .AddIf(Link, "is-link")
                .AddIf(Bg, "is-has-bg")
                .AddIf(Dashed, "is-dashed");
                return;
            }
            cssClassBuilder.AddIf(!string.IsNullOrWhiteSpace(Cls), Cls);
            if (string.IsNullOrWhiteSpace(Cls))
            {
                cssClassBuilder.Add("el-button")
                    .AddIf(TypeCssValue != null, $"el-button--{TypeCssValue}")
                    .AddIf(SizeCssValue != null, $"el-button--{SizeCssValue}")
                    .AddIf(Plain, "is-plain")
                    .AddIf(Round, "is-round")
                    .AddIf(IsButtonDisabled, "is-disabled")
                    .AddIf(Loading, "is-loading")
                    .AddIf(Circle, "is-circle")
                    .AddIf(IsTextButton, "is-text")
                    .AddIf(Link, "is-link")
                    .AddIf(Bg, "is-has-bg")
                    .AddIf(Dashed, "is-dashed");
            }
        }

        protected RenderFragment ButtonElement => builder =>
        {
            var seq = 0;
            builder.OpenElement(seq++, EffectiveTag);
            if (Attributes != null)
            {
                builder.AddMultipleAttributes(seq++, Attributes);
            }

            builder.AddAttribute(seq++, "style", buttonStyleBuilder);
            builder.AddAttribute(seq++, "class", cssClassBuilder);
            builder.AddAttribute(seq++, "aria-disabled", IsButtonDisabled ? "true" : "false");

            if (Loading)
            {
                builder.AddAttribute(seq++, "aria-busy", "true");
            }

            if (Autofocus)
            {
                builder.AddAttribute(seq++, "autofocus", true);
            }

            if (IsNativeButton)
            {
                builder.AddAttribute(seq++, "type", string.IsNullOrWhiteSpace(NativeType) ? "button" : NativeType);
                builder.AddAttribute(seq++, "disabled", IsButtonDisabled);
            }
            else
            {
                builder.AddAttribute(seq++, "role", "button");
                builder.AddAttribute(seq++, "tabindex", IsButtonDisabled ? -1 : Tabindex ?? 0);
                builder.AddAttribute(seq++, "onkeydown", EventCallback.Factory.Create<KeyboardEventArgs>(this, OnButtonKeyDownAsync));
                if (string.Equals(EffectiveTag, "a", StringComparison.OrdinalIgnoreCase))
                {
                    builder.AddAttribute(seq++, "href", IsButtonDisabled ? null : Href);
                    builder.AddAttribute(seq++, "target", Target);
                    builder.AddAttribute(seq++, "rel", Rel);
                }
            }

            builder.AddAttribute(seq++, "onclick", EventCallback.Factory.Create<MouseEventArgs>(this, OnButtonClickedAsync));
            BuildButtonContent(builder, ref seq);
            builder.CloseElement();
        };

        private void BuildButtonContent(RenderTreeBuilder builder, ref int seq)
        {
            if (Loading)
            {
                if (LoadingContent != null)
                {
                    builder.AddContent(seq++, LoadingContent);
                }
                else if (!string.IsNullOrWhiteSpace(LoadingIcon))
                {
                    builder.OpenElement(seq++, "i");
                    builder.AddAttribute(seq++, "class", LoadingIcon);
                    builder.AddAttribute(seq++, "aria-hidden", "true");
                    builder.CloseElement();
                }
            }
            else if (IconContent != null)
            {
                builder.AddContent(seq++, IconContent);
            }
            else if (!string.IsNullOrWhiteSpace(Icon))
            {
                builder.OpenElement(seq++, "i");
                builder.AddAttribute(seq++, "class", Icon);
                builder.AddAttribute(seq++, "aria-hidden", "true");
                builder.CloseElement();
            }

            if (!string.IsNullOrWhiteSpace(RenderedButtonTextContent) || ChildContent != null)
            {
                builder.OpenElement(seq++, "span");
                builder.AddContent(seq++, RenderedButtonTextContent);
                builder.AddContent(seq++, ChildContent);
                builder.CloseElement();
            }
        }

        private async Task OnButtonKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key != "Enter" && e.Key != " ")
            {
                return;
            }

            await OnButtonClickedAsync(new MouseEventArgs());
        }
    }
}

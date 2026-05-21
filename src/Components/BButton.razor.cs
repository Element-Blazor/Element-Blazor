using Microsoft.AspNetCore.Components;
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
        protected async Task OnButtonClickedAsync(MouseEventArgs e)
        {
            if (Disabled || Loading)
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

        protected string ButtonTextContent => Content ?? Label;

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
                .AddIf(!string.IsNullOrWhiteSpace(Color), $"--el-button-bg-color:{Color}", $"--el-button-border-color:{Color}");
            if (string.IsNullOrWhiteSpace(Cls) || AppendCustomCls)
            {
                cssClassBuilder.Add("el-button", Cls)
                .AddIf(Type != ButtonType.Default, $"el-button--{Type.ToString().ToLower()}")
                .AddIf(Size != ButtonSize.Default, $"el-button--{Size.ToString().ToLower()}")
                .AddIf(Plain, "is-plain")
                .AddIf(Round, "is-round")
                .AddIf(Disabled, "is-disabled")
                .AddIf(Loading, "is-loading")
                .AddIf(Circle, "is-circle")
                .AddIf(Text, "is-text")
                .AddIf(Link, "is-link")
                .AddIf(Bg, "is-has-bg")
                .AddIf(Dashed, "is-dashed");
                return;
            }
            cssClassBuilder.AddIf(!string.IsNullOrWhiteSpace(Cls), Cls);
            if (string.IsNullOrWhiteSpace(Cls))
            {
                cssClassBuilder.Add("el-button")
                    .AddIf(Type != ButtonType.Default, $"el-button--{Type.ToString().ToLower()}")
                    .AddIf(Size != ButtonSize.Default, $"el-button--{Size.ToString().ToLower()}")
                    .AddIf(Plain, "is-plain")
                    .AddIf(Round, "is-round")
                    .AddIf(Disabled, "is-disabled")
                    .AddIf(Loading, "is-loading")
                    .AddIf(Circle, "is-circle")
                    .AddIf(Text, "is-text")
                    .AddIf(Link, "is-link")
                    .AddIf(Bg, "is-has-bg")
                    .AddIf(Dashed, "is-dashed");
            }
        }
    }
}

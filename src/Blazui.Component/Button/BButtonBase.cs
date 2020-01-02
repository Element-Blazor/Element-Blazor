using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Button
{
    public class BButtonBase : BComponentBase
    {
        internal HtmlPropertyBuilder cssClassBuilder;
        protected async Task OnButtonClickedAsync(MouseEventArgs e)
        {
            if (IsDisabled)
            {
                return;
            }
            if (OnClick.HasDelegate)
            {
                await OnClick.InvokeAsync(e);
            }
        }

        [Parameter]
        public string Cls { get; set; }

        [Parameter]
        public EventCallback<MouseEventArgs> OnClick { get; set; }

        [Parameter]
        public ButtonType Type { get; set; } = ButtonType.Default;

        [Parameter]
        public bool IsPlain { get; set; }

        [Parameter]
        public bool IsRound { get; set; }

        [Parameter]
        public bool IsDisabled { get; set; }

        [Parameter]
        public bool IsCircle { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public ButtonSize Size { get; set; }

        [Parameter]
        public string Icon { get; set; }

        [Parameter]
        public bool IsLoading { get; set; }

        protected override bool ShouldRender()
        {
            return true;
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (IsLoading)
            {
                IsDisabled = true;
            }
            cssClassBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add($"el-button", $"el-button--{Type.ToString().ToLower()}", Cls)
                .AddIf(Size != ButtonSize.Default, $"el-button--{Size.ToString().ToLower()}")
                .AddIf(IsPlain, "is-plain")
                .AddIf(IsRound, "is-round")
                .AddIf(IsDisabled, "is-disabled")
                .AddIf(IsLoading, "is-loading")
                .AddIf(IsCircle, "is-circle");
        }
    }
}

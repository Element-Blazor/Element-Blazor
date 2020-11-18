using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;

namespace Element
{
    public partial class BLayout : BComponentBase, IContainerComponent
    {
        [Parameter]
        public bool Fit { get; set; } = true;
        [Parameter]
        public float Width { get; set; }
        [Parameter]
        public float Height { get; set; }

        [Parameter]
        public float WestWidth { get; set; } = 200;
        [Parameter]
        public float EastWidth { get; set; } = 200;
        [Parameter]
        public float NorthHeight { get; set; } = 50;
        [Parameter]
        public float SouthHeight { get; set; } = 50;
        [Parameter]
        public RenderFragment West { get; set; }

        [Parameter]
        public RenderFragment East { get; set; }

        [Parameter]
        public RenderFragment Center { get; set; }

        [Parameter]
        public RenderFragment North { get; set; }

        [Parameter]
        public RenderFragment South { get; set; }

        /// <summary>
        /// 用于捕获 DOM 节点以用于展示 Loading
        /// </summary>
        public ElementReference Container { get; set; }

        internal HtmlPropertyBuilder layoutCssBuilder;
        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            layoutCssBuilder = HtmlPropertyBuilder.CreateCssStyleBuilder().AddIf(Fit, "height:calc(100vh)").Add(Style);
        }

        /// <summary>
        /// 键按下时触发
        /// </summary>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyDown { get; set; }

        /// <summary>
        /// 键弹起时触发
        /// </summary>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyUp { get; set; }

        /// <summary>
        /// 按键时触发
        /// </summary>
        [Parameter]
        public EventCallback<KeyboardEventArgs> OnKeyPress { get; set; }

        protected internal Task KeyDown(KeyboardEventArgs e)
        {
            return OnKeyDown.InvokeAsync(e);
        }
        protected internal Task KeyPress(KeyboardEventArgs e)
        {
            return OnKeyPress.InvokeAsync(e);
        }
        protected internal Task KeyUp(KeyboardEventArgs e)
        {
            return OnKeyUp.InvokeAsync(e);
        }
    }
}

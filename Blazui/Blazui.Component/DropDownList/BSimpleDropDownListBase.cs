using Blazui.Component.Dom;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.DropDownList
{
    public class BSimpleDropDownListBase : ComponentBase
    {
        [Inject]
        private IJSRuntime jSRuntime { get; set; }
        protected BSimpleDropDownListContent content;
        [Parameter]
        public string Placeholder { get; set; } = "请选择";

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        private bool isShow;

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var seq = 0;
            builder.OpenComponent<BSimpleDropDownListSelect>(seq++);
            builder.AddAttribute(seq++, "OnClick", OnInternalClick);
            builder.AddAttribute(seq++, "Placeholder", Placeholder);
            builder.CloseComponent();
            builder.OpenComponent<BSimpleDropDownListContent>(seq++);
            builder.AddComponentReferenceCapture(seq++, c => content = (BSimpleDropDownListContent)c);
            builder.AddAttribute(seq++, "ZIndex", ComponentManager.GenerateZIndex());
            builder.AddAttribute(seq++, "IsShow", isShow);
            builder.AddAttribute(seq++, "ChildContent", ChildContent);
            builder.CloseComponent();
        }

        private void OnInternalClick(UIMouseEventArgs e)
        {
            var element = content.Element.Dom(jSRuntime);
            //element

        }
    }
}

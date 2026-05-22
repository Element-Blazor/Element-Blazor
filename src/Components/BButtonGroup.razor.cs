using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class BButtonGroup
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public ButtonSize Size { get; set; }

        [Parameter]
        public ButtonType Type { get; set; } = ButtonType.Default;

        [Parameter]
        public string Direction { get; set; } = "horizontal";

        protected string GroupClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-button-group", Cls)
            .AddIf(string.Equals(Direction, "vertical", StringComparison.OrdinalIgnoreCase), "is-vertical")
            .ToString();
    }
}

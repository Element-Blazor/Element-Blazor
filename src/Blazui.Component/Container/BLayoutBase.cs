using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazui.Component.Container
{
    public class BLayoutBase : BComponentBase, IContainerComponent
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
        public float SouthWidth { get; set; } = 200;
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
    }
}

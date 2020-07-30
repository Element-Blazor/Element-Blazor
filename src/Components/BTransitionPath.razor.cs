using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component
{
    public partial class BTransitionPath
    {
        [CascadingParameter]
        public BTransition Transition { get; set; }

        /// <summary>
        /// 是否增量应用样式，而不是最终的样式
        /// </summary>
        [Parameter]
        public bool Increment { get; set; }

        /// <summary>
        /// 动画执行时长，以毫秒为单位
        /// </summary>
        [Parameter]
        public int Duration { get; set; }

        /// <summary>
        /// 延迟执行，以毫秒为单位
        /// </summary>
        [Parameter]
        public int Delay { get; set; }
        protected override void OnInitialized()
        {
            Transition.AddPathConfig(Style, Delay, Increment, Duration);
        }
    }
}

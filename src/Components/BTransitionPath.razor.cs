using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
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
        /// 使动画暂停执行，直到 <see cref="BTransition"/> 的 <see cref="BTransition.Resume"/> 方法被调用
        /// </summary>
        [Parameter]
        public bool? Pause { get; set; }

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
            var option = new TransitionOption();
            option.Style = string.IsNullOrWhiteSpace(Style) ? string.Join(";", Attributes.Select(x => $"{x.Key}:{x.Value.ToString()}")) : Style;
            option.Delay = Delay;
            option.Increment = Increment;
            option.Duration = Duration;
            option.Pause = Pause;
            Transition.AddPathConfig(option);
        }
    }
}

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BPaginationBase : ComponentBase
    {
        internal bool previousDisabled = false;
        internal bool nextDisabled = false;
        internal bool nextArrow = false;
        internal bool prevArrow = false;
        /// <summary>
        /// 总记录数
        /// </summary>
        [Parameter]
        public int Total { get; set; } = 100;

        internal void ShowNext()
        {
            nextArrow = true;
            prevArrow = false;
        }
        internal void HideNext()
        {
            nextArrow = false;
        }

        internal void ShowPrev()
        {
            prevArrow = true;
        }
        internal void HidePrev()
        {
            prevArrow = false;
        }

        /// <summary>
        /// 每页条数
        /// </summary>
        [Parameter]
        public int PageSize { get; set; } = 50;
        /// <summary>
        /// 当前页码，从1开始
        /// </summary>
        [Parameter]
        public int CurrentPage { get; set; } = 1;

        /// <summary>
        /// 是否显示背景颜色
        /// </summary>
        [Parameter]
        public bool Background { get; set; } = true;

        /// <summary>
        /// 最大显示的页码数
        /// </summary>
        [Parameter]
        public int ShowPageCount { get; set; } = 7;

        /// <summary>
        /// 当前页码变化时触发
        /// </summary>
        [Parameter]
        public Func<int, Task> CurrentPageChanged { get; set; }

        /// <summary>
        /// 当前最大显示的页码数变化时触发
        /// </summary>
        [Parameter]
        public EventCallback<int> PageCountChanged { get; set; }

        internal int pageCount;

        protected override void OnParametersSet()
        {
            pageCount = Convert.ToInt32(Math.Ceiling((float)Total / PageSize));
            if (CurrentPage > pageCount)
            {
                CurrentPage = pageCount;
                CurrentPageChanged?.Invoke(pageCount);
            }
            SwitchButtonStatus();
        }

        internal void Jump(int page)
        {
            CurrentPage = page;
            if (CurrentPageChanged != null)
            {
                _ = CurrentPageChanged(page);
            }
            SwitchButtonStatus();
        }

        private void SwitchButtonStatus()
        {
            if (pageCount <= 1)
            {
                previousDisabled = true;
                nextDisabled = true;
                return;
            }
            if (CurrentPage <= 1)
            {
                previousDisabled = true;
                nextDisabled = false;
            }
            else if (CurrentPage >= pageCount)
            {
                previousDisabled = false;
                nextDisabled = true;
            }
            else
            {
                previousDisabled = nextDisabled = false;
            }
        }
    }
}

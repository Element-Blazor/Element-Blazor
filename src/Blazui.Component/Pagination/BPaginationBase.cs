using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Pagination
{
    public class BPaginationBase : ComponentBase
    {
        protected bool previousDisabled = false;
        protected bool nextDisabled = false;
        /// <summary>
        /// 总记录数
        /// </summary>
        [Parameter]
        public int Total { get; set; } = 100;

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

        [Parameter]
        public bool Background { get; set; } = true;

        [Parameter]
        public EventCallback<int> CurrentPageChanged { get; set; }
        protected int pageCount;
        protected override void OnInitialized()
        {
            pageCount = Convert.ToInt32(Math.Ceiling((float)Total / PageSize));
            Calculate();
        }

        protected void Jump(int page)
        {
            CurrentPage = page;
            Calculate();
        }

        private void Calculate()
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

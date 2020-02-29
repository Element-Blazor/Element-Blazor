using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BStepBase : BComponentBase
    {
        /// <summary>
        /// 标题
        /// </summary>
        [Parameter]
        public string Title { get; set; }

        /// <summary>
        /// 简短的描述
        /// </summary>
        [Parameter]
        public string Description { get; set; }
    }
}

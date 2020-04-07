using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BCardBase : BComponentBase, IContainerComponent
    {
        internal HtmlPropertyBuilder wrapperClassBuilder;
        internal HtmlPropertyBuilder bodyClassBuilder;

        /// <summary>
        /// Body 上的 Class 类
        /// </summary>
        [Parameter]
        public string BodyCls { get; set; }

        /// <summary>
        /// 阴影类型
        /// </summary>
        [Parameter]
        public ShadowShowType Shadow { get; set; } = ShadowShowType.Always;

        [Parameter]
        public RenderFragment Header { get; set; }

        [Parameter]
        public RenderFragment Body { get; set; }

        public ElementReference Container { get; set; }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            wrapperClassBuilder = CssClassBuilder.CreateCssClassBuilder().Add("el-card", "box-card", $"is-{Shadow.ToString().ToLower()}-shadow", Cls);
            bodyClassBuilder = CssClassBuilder.CreateCssClassBuilder().Add("el-card__body", BodyCls);
        }
    }
}

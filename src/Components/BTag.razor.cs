using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class BTag
    {
        private bool isClosed = false;
        private HtmlPropertyBuilder iconCssBuilder;
        private HtmlPropertyBuilder tagCssBuilder;
        private HtmlPropertyBuilder tagStyleBuilder;

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        /// <summary>
        /// 是否可关闭
        /// </summary>
        [Parameter]
        public bool Closable { get; set; }

        /// <summary>
        /// 标签类型
        /// </summary>
        [Parameter]
        public TagType Type { get; set; }

        /// <summary>
        /// 主题
        /// </summary>
        [Parameter]
        public TagTheme Theme { get; set; }
        /// <summary>
        /// 点击事件
        /// </summary>
        [Parameter]
        public EventCallback OnClick { get; set; }

        /// <summary>
        /// 关闭前执行
        /// </summary>
        [Parameter]
        public Func<Task<bool>> OnBeforeClose { get; set; }

        /// <summary>
        /// 关闭后执行
        /// </summary>
        [Parameter]
        public EventCallback OnAfteClose { get; set; }

        /// <summary>
        /// 尺寸
        /// </summary>
        [Parameter]
        public TagSize Size { get; set; }

        private async Task OnTagClick()
        {
            if (!OnClick.HasDelegate)
            {
                return;
            }

            await OnClick.InvokeAsync(null);
        }
        private async Task OnCloseClick()
        {
            if (OnBeforeClose != null && !await OnBeforeClose())
            {
                return;
            }

            isClosed = true;
            StateHasChanged();
            if (OnAfteClose.HasDelegate)
            {
                await OnAfteClose.InvokeAsync(null);
            }
        }

        protected override void OnParametersSet()
        {
            iconCssBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .AddIf(Closable, "el-tag__close", "el-icon-close");
            tagCssBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                .Add("el-tag", $"el-tag--{Size.ToString().ToLower()}", $"el-tag--{Theme.ToString().ToLower()}", $"el-tag--{Type.ToString().ToLower()}");
            tagStyleBuilder = HtmlPropertyBuilder.CreateCssStyleBuilder()
                .AddIf(!string.IsNullOrEmpty(Style), Style.Split(';'));
        }
    }
}

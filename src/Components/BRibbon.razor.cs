using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Element
{
    public partial class BRibbon
    {
        private HtmlPropertyBuilder styleBuilder;
        private HtmlPropertyBuilder menuStyleBuilder;
        private BTabPanel menuTabPanel;

        [Parameter]
        public Type Menu { get; set; }
        [Parameter]
        public RenderFragment Tabs { get; set; }

        /// <summary>
        /// 布局内容区
        /// </summary>
        [Parameter]
        public RenderFragment Body { get; set; }
        /// <summary>
        /// 菜单背景颜色
        /// </summary>
        [Parameter]
        public string MenuBackgroundColor { get; set; }

        /// <summary>
        /// 菜单字体颜色
        /// </summary>
        [Parameter]
        public string MenuColor { get; set; }

        /// <summary>
        /// 菜单文字
        /// </summary>
        [Parameter]
        public string MenuText { get; set; }
        /// <summary>
        /// 导航区高度
        /// </summary>
        [Parameter]
        public int NavigationHeight { get; set; }

        protected override void OnParametersSet()
        {
            styleBuilder = HtmlPropertyBuilder.CreateCssStyleBuilder()
                .AddIf(NavigationHeight <= 0, "height:160px")
                .AddIf(NavigationHeight > 0, $"height:{NavigationHeight.ToString()}px");
            menuStyleBuilder = HtmlPropertyBuilder.CreateCssStyleBuilder()
                .AddIf(string.IsNullOrEmpty(MenuBackgroundColor), "background-color:#0062AF;")
                .AddIf(!string.IsNullOrEmpty(MenuBackgroundColor), $"background-color:{MenuBackgroundColor}")
                .AddIf(!string.IsNullOrEmpty(MenuText), $"color:{MenuText}")
                .AddIf(string.IsNullOrEmpty(MenuText), "color:#fff;")
                .Add("border-radus:3px");
        }

        private async System.Threading.Tasks.Task PreventMenuSwitchAsync(BChangeEventArgs<BTabPanel> eventArg)
        {
            //if (eventArg.NewValue.Title == MenuText)
            //{
            //    await ShowMenuAsync(eventArg);
            //}
            //else if (string.IsNullOrWhiteSpace(MenuText) && eventArg.NewValue.Title == "开始")
            //{
            //    await ShowMenuAsync(eventArg);
            //}
        }

        private async Task ShowMenuAsync(BChangeEventArgs<BTabPanel> eventArg)
        {
            eventArg.DisallowChange = true;
            var rect = await menuTabPanel.Element.Dom(JSRuntime).GetBoundingClientRectAsync();
            var point = new System.Drawing.PointF(rect.X, rect.Height);
            await DialogService.ShowLayerAsync<DialogResult>(Menu, 0, new Dictionary<string, object>(), point);
        }
    }
}

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace Element
{
    public partial class BPopupLayer
    {
        private ElementReference container;
        private bool rendered = false;
        private PopupLayerOption popupLayerOption;

        [Inject]
        private PopupService popupService { get; set; }
        /// <summary>
        /// 显示到什么元素上
        /// </summary>
        [Parameter]
        public ElementReference ShowTo { get; set; }

        /// <summary>
        /// 显示位置
        /// </summary>
        [Parameter]
        public ShowToPosition Position { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (string.IsNullOrWhiteSpace(ShowTo.Id))
            {
                if (!rendered)
                {
                    await Task.Delay(100);
                }
                return;
            }
            if (rendered)
            {
                return;
            }
            rendered = true;
            var showToRect = await ShowTo.Dom(JSRuntime).GetBoundingClientRectAsync();
            var containerRect = await container.Dom(JSRuntime).GetBoundingClientRectAsync();
            popupLayerOption = new PopupLayerOption();
            popupLayerOption.Content = ChildContent;
            switch (Position)
            {
                case ShowToPosition.Bottom:
                    popupLayerOption.Position = new System.Drawing.PointF()
                    {
                        X = (showToRect.X + showToRect.Width - containerRect.Width) / 2,
                        Y = showToRect.Y + showToRect.Height
                    };
                    break;
                case ShowToPosition.Top:
                    popupLayerOption.Position = new System.Drawing.PointF()
                    {
                        X = (showToRect.X + showToRect.Width - containerRect.Width) / 2,
                        Y = showToRect.Y - 32/*按钮高度*/
                    };
                    break;
            }
            ChildContent = null;
            MarkAsRequireRender();
            StateHasChanged();
            popupService.PopupLayerOptions.Add(popupLayerOption);
        }

        public override void Dispose()
        {
            popupService.PopupLayerOptions.Remove(popupLayerOption);
        }
    }
}

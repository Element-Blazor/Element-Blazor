using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Blazui.Component.EventArgs;
using Microsoft.AspNetCore.Components;

namespace Blazui.Component.Container
{
    /// <summary>
    /// Tab 选项
    /// </summary>
    public class TabOption
    {
        /// <summary>
        /// Tab 名称
        /// </summary>
        public string Name { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// 是否可关闭
        /// </summary>
        public bool IsClosable { get; set; }

        /// <summary>
        /// 是否禁用
        /// </summary>
        public bool IsDisabled { get; set; }

        /// <summary>
        /// Tab 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Tag 内容，可以是一串字符串、RenderFragment 以及一个组件的 Type
        /// </summary>
        public object Content { get; set; }

        /// <summary>
        /// 是否活动
        /// </summary>
        public bool IsActive { get; set; }

        /// <summary>
        /// Tab 页渲染完成时触发
        /// </summary>
        public EventCallback<BSimpleTabPanelBase> OnRenderCompleted { get; set; }

        /// <summary>
        /// Tab 页切换时触发
        /// </summary>
        public EventCallback<BChangeEventArgs<BSimpleTabPanelBase>> OnTabPanelChanging { get; set; }
    }
}


using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class PopupOption
    {
        /// <summary>
        /// 关联到的 BPopup 组件实例
        /// </summary>
        public BPopup Instance { get; internal set; }

        /// <summary>
        /// 弹出层元素本身
        /// </summary>
        internal ElementReference Element { get; set; }

        /// <summary>
        /// 遮罩层元素
        /// </summary>
        internal ElementReference ShadowElement { get; set; }

        /// <summary>
        /// 要将弹出层显示到哪个元素下方
        /// </summary>
        internal ElementReference Target { get; set; }

        /// <summary>
        /// 隐藏动画状态
        /// </summary>
        internal AnimationStatus HideStatus { get; set; }

        /// <summary>
        /// 显示动画状态
        /// </summary>
        internal AnimationStatus ShowStatus { get; set; }

        internal bool IsShow { get; set; }

        internal float Top { get; set; }
        internal float Left { get; set; }
        public bool IsNew { get; internal set; }
        internal int ZIndex { get; set; }
        internal int ShadowZIndex { get; set; }
    }
}

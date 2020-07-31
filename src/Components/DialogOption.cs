
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class DialogOption
    {
        public string Title { get; set; }
        public object Content { get; set; }

        /// <summary>
        /// 窗口要显示的组件所需要的参数
        /// </summary>
        public IDictionary<string, object> Parameters { get; set; }

        /// <summary>
        /// 是否窗口
        /// </summary>
        public bool IsDialog { get; set; }
        /// <summary>
        /// 弹窗全屏
        /// </summary>
        public bool FullScreen { get; set; } = false;
        public DialogResult Result { get; internal set; }

        /// <summary>
        /// 宽度
        /// </summary>
        public float Width { get; set; }

        /// <summary>
        /// 窗口按钮，尚未测试
        /// </summary>
        public IList<RenderFragment> Buttons { get; set; } = new List<RenderFragment>();

        /// <summary>
        /// 是否模态窗口
        /// </summary>
        public bool IsModal { get; set; } = true;

        /// <summary>
        /// 窗口显示位置
        /// </summary>
        public PointF Point { get; internal set; }
        internal int ZIndex { get; set; }
        internal int ShadowZIndex { get; set; }
        internal TaskCompletionSource<DialogResult> TaskCompletionSource { get; set; }
        internal BPopup Instance { get; set; }
        internal BTransition Element { get; set; }
        internal BTransition ShadowElement { get; set; }
        internal bool IsNew { get; set; }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="result"></param>
        /// <returns></returns>
        public Task CloseDialogAsync<T>(T result)
        {
            Instance.CloseDialogAsync(this, new DialogResult()
            {
                Result = result
            });
            return Task.CompletedTask;
        }

        /// <summary>
        /// 关闭窗口
        /// </summary>
        /// <returns></returns>
        public async Task CloseDialogAsync()
        {
            await CloseDialogAsync((object)null);
        }
    }
}

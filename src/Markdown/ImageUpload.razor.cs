using Blazui.Component;

using Blazui.Markdown.IconHandlers;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Markdown
{
    public partial class ImageUpload
    {
        internal protected BForm form;

        [Parameter]
        public ImageModel Image { get; set; }

        /// <summary>
        /// 文本提示
        /// </summary>
        [Parameter]
        public string Tip { get; set; }

        /// <summary>
        /// 禁用上传
        /// </summary>
        [Parameter]
        public bool DisableUpload { get; set; }
        [Parameter]
        public string UploadUrl { get; set; }

        /// <summary>
        /// 单文件最大限制，KB为单位
        /// </summary>
        [Parameter]
        public long MaxSize { get; set; }

        /// <summary>
        /// 图片最大宽度
        /// </summary>
        [Parameter]
        public float Width { get; set; }

        /// <summary>
        /// 图片最大高度
        /// </summary>
        [Parameter]
        public float Height { get; set; }

        /// <summary>
        /// 允许上传的文件后缀
        /// </summary>
        [Parameter]
        public string[] AllowExtensions { get; set; }
        internal protected void Submit()
        {
            if (!form.IsValid())
            {
                return;
            }
            _ = DialogService.CloseDialogAsync(this, form.GetValue<ImageModel>());
        }
    }
}

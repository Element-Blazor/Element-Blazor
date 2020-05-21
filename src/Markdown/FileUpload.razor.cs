using Blazui.Component;

using Blazui.Markdown.IconHandlers;
using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Markdown
{
    public partial class FileUpload
    {
        internal protected BForm form;

        /// <summary>
        /// 文件上传地址
        /// </summary>
        [Parameter]
        public string UploadUrl { get; set; }

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

        /// <summary>
        /// 单文件最大限制，KB为单位
        /// </summary>
        [Parameter]
        public long MaxSize { get; set; }

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
            _ = DialogService.CloseDialogAsync(this, form.GetValue<FileModel>());
        }
    }
}

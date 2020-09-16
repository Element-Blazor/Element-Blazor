using Blazui.Component;

using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Blazui.Markdown
{
    public partial class BMarkdownFileUpload
    {
        /// <summary>
        /// 上传地址
        /// </summary>
        [Parameter]
        public string UploadUrl { get; set; }

        /// <summary>
        /// 禁用上传
        /// </summary>
        [Parameter]
        public bool DisableUpload { get; set; }

        /// <summary>
        /// 文本提示
        /// </summary>
        [Parameter]
        public string Tip { get; set; }

        /// <summary>
        /// 上传类型
        /// </summary>
        [Parameter]
        public UploadType UploadType { get; set; }

        /// <summary>
        /// 文件地址列表
        /// </summary>
        [Parameter]
        public string[] Urls { get; set; }

        /// <summary>
        /// 单文件最大限制，KB为单位
        /// </summary>
        [Parameter]
        public long MaxSize { get; set; }

        /// <summary>
        /// 如果上传的是图片，指定图片最大宽度
        /// </summary>
        [Parameter]
        public float Width { get; set; }

        /// <summary>
        /// 如果上传的是图片，指定图片最大高度
        /// </summary>
        [Parameter]
        public float Height { get; set; }

        /// <summary>
        /// 允许上传的文件后缀
        /// </summary>
        [Parameter]
        public string[] AllowExtensions { get; set; }
        [Parameter]
        public EventCallback<string[]> UrlsChanged { get; set; }

        protected internal void UpdateFiles(IFileModel[] files)
        {
            Urls = files.Select(x => x.Url).ToArray();
            if (UrlsChanged.HasDelegate)
            {
                _ = UrlsChanged.InvokeAsync(Urls);
            }
            RequireRender = true;
            SetFieldValue(Urls, true);
        }
        protected internal void OnDeleteFile(IFileModel file)
        {
            Urls = Urls.Where(x => x != file.Url).ToArray();
            if (UrlsChanged.HasDelegate)
            {
                _ = UrlsChanged.InvokeAsync(Urls);
            }
            RequireRender = true;
            SetFieldValue(Urls, true);
        }
        protected internal void UpdateUrl(string file)
        {
            Urls = new string[] { file };
            if (UrlsChanged.HasDelegate)
            {
                _ = UrlsChanged.InvokeAsync(Urls);
            }
            RequireRender = true;
            SetFieldValue(Urls, true);
        }
    }
}

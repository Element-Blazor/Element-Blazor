using System;
using System.Collections.Generic;
using System.Text;

namespace Blazui.Component.ControlConfigs
{
    public class UploadAttribute : Attribute
    {
        /// <summary>
        /// 上传地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 上传类型
        /// </summary>
        public UploadType Type { get; set; }

        /// <summary>
        /// 大小限制，以 KB 为单位
        /// </summary>
        public long MaxSize { get; set; }

        /// <summary>
        /// 粘贴上传
        /// </summary>
        public bool EnablePasteUpload { get; set; } = true;

        /// <summary>
        /// 上传提示
        /// </summary>
        public string Tip { get; set; }

        /// <summary>
        /// 允许的后缀
        /// </summary>
        public string[] AllowExtensions { get; set; } = new string[0];

        /// <summary>
        /// 对图片文件限制宽度
        /// </summary>
        public float Width { get; set; }
        /// <summary>
        /// 对图片文件限制高度
        /// </summary>
        public float Height { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Text;

namespace Element.ControlConfigs
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
        /// 列表类型
        /// </summary>
        public UploadListType ListType { get; set; } = UploadListType.Text;

        /// <summary>
        /// 拖拽上传
        /// </summary>
        public bool Drag { get; set; }

        /// <summary>
        /// 文件数量限制
        /// </summary>
        public int Limit { get; set; }

        /// <summary>
        /// 是否允许多选
        /// </summary>
        public bool Multiple { get; set; } = true;

        /// <summary>
        /// 选择文件后自动上传
        /// </summary>
        public bool AutoUpload { get; set; } = true;

        /// <summary>
        /// 显示文件列表
        /// </summary>
        public bool ShowFileList { get; set; } = true;

        /// <summary>
        /// 禁用上传
        /// </summary>
        public bool Disabled { get; set; }

        /// <summary>
        /// 原生 accept 属性
        /// </summary>
        public string Accept { get; set; }

        /// <summary>
        /// 请求方法
        /// </summary>
        public string Method { get; set; } = "post";

        /// <summary>
        /// 上传文件字段名
        /// </summary>
        public string FileFieldName { get; set; } = "fileContent";

        /// <summary>
        /// 携带凭据
        /// </summary>
        public bool WithCredentials { get; set; } = true;

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

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class UploadModel: IFileModel
    {
        /// <summary>
        /// 文件名
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public UploadStatus Status { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 文件唯一标识
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();
        /// <summary>
        /// 上传地址返回的 message 字段信息
        /// </summary>
        public string Message { get; set; }

        public override int GetHashCode()
        {
            return FileName?.GetHashCode() ?? 0;
        }
    }
}

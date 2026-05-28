using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
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

        /// <summary>
        /// 文件大小，单位为 byte
        /// </summary>
        public long Size { get; set; }

        /// <summary>
        /// 图片宽度
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 图片高度
        /// </summary>
        public int Height { get; set; }

        public override int GetHashCode()
        {
            return FileName?.GetHashCode() ?? 0;
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public enum UploadStatus
    {
        /// <summary>
        /// 未开始
        /// </summary>
        UnStart = 0,

        /// <summary>
        /// 上传中
        /// </summary>
        Uploading = 1,

        /// <summary>
        /// 上传成功
        /// </summary>
        Success = 2,

        /// <summary>
        /// 上传失败
        /// </summary>
        Failure = 3
    }
}

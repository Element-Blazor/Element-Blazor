using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public interface IFileModel
    {
        /// <summary>
        /// 文件名
        /// </summary>
        string FileName { get; set; }
        /// <summary>
        /// 文件唯一标识
        /// </summary>
        string Id { get; set; }
        /// <summary>
        /// 文件访问地址，如果上传的是图片，并且需要预览，则必须传入访问地址
        /// </summary>
        string Url { get; set; }
    }
}

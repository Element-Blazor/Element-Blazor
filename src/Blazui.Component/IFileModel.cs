using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
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
        /// 图片地址
        /// </summary>
        string Url { get; set; }
    }
}

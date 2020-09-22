using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    /// <summary>
    /// 树控件数据源的格式
    /// </summary>
    public enum DataFormat
    {
        /// <summary>
        /// 没有层级关系，所有的节点都在一个集合当中
        /// </summary>
        List = 0,

        /// <summary>
        /// 每一个节点对应的数据都有标识自己的子节点，有层级关系
        /// </summary>
        Children = 1
    }
}

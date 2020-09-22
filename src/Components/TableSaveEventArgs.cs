using System;
using System.Collections.Generic;
using System.Text;

namespace Element
{
    public class TableSaveEventArgs
    {
        /// <summary>
        /// 保存动作类型
        /// </summary>
        public SaveAction Action { get; set; }

        /// <summary>
        /// 要保存的数据
        /// </summary>
        public object Data { get; set; }

        /// <summary>
        /// 是否取消保存动作
        /// </summary>
        public bool Cancel { get; set; }

        /// <summary>
        /// 表格对象
        /// </summary>
        public BTable Table { get; set; }
        public string Key { get; set; }
        public Type DataType { get; set; }
    }
}

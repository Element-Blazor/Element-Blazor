using System;
using System.Collections.Generic;
using System.Text;

namespace Element.Admin
{
    public class OperationResult
    {
        /// <summary>
        /// 响应码，为0表示成功
        /// </summary>
        public int Code { get; set; }

        /// <summary>
        /// 响应消息
        /// </summary>
        public string Message { get; set; }
    }
}

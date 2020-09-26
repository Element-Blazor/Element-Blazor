using System;
using System.Collections.Generic;
using System.Text;

namespace Element.Admin
{
    /// <summary>
    /// 当用户操作引发业务异常时触发，异常消息将显示给用户
    /// </summary>
    public class OperationException : Exception
    {
        public OperationException(string message) : base(message)
        {

        }
    }
}

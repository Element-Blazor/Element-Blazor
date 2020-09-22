using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    /// <summary>
    /// CSS 类构建器
    /// </summary>
    public class CssClassBuilder : HtmlPropertyBuilder
    {
        public override string ToString()
        {
            return base.ToString(" ");
        }
    }
}

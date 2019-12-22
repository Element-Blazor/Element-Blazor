using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    /// <summary>
    /// CSS 构建器
    /// </summary>
    public class CssBuilder
    {
        private List<string> styles = new List<string>();
        /// <summary>
        /// 创建一个 <seealso cref="CssBuilder"/>
        /// </summary>
        /// <returns></returns>
        public static CssBuilder Create()
        {
            return new CssBuilder();
        }

        /// <summary>
        /// 如果满足条件，则添加一个属性
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="css">css 属性</param>
        /// <returns></returns>
        public CssBuilder AddIf(bool condition, string css)
        {
            if (!condition)
            {
                return this;
            }
            styles.Add(css);
            return this;
        }

        public override string ToString()
        {
            return string.Join(";", styles);
        }
    }
}

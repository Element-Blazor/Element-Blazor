using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    /// <summary>
    /// 属性构建器
    /// </summary>
    public class HtmlPropertyBuilder
    {
        private List<string> properties = new List<string>();
        /// <summary>
        /// 创建一个 <seealso cref="HtmlPropertyBuilder"/>
        /// </summary>
        /// <returns></returns>
        public static HtmlPropertyBuilder Create()
        {
            return new HtmlPropertyBuilder();
        }

        /// <summary>
        /// 如果满足条件，则添加一个属性
        /// </summary>
        /// <param name="condition">条件</param>
        /// <param name="propertyList">css 属性</param>
        /// <returns></returns>
        public HtmlPropertyBuilder AddIf(bool condition, params string[] propertyList)
        {
            if (!condition)
            {
                return this;
            }
            properties.AddRange(propertyList);
            return this;
        }

        /// <summary>
        /// 添加一个属性
        /// </summary>
        /// <param name="cssList">css 属性</param>
        /// <returns></returns>
        public HtmlPropertyBuilder Add(params string[] cssList)
        {
            return AddIf(true, cssList);
        }

        internal string ToString(string sep)
        {
            return string.Join(sep, properties.Where(x => !string.IsNullOrWhiteSpace(x)));
        }
    }
}

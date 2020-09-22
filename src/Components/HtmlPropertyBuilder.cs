using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    /// <summary>
    /// 属性构建器
    /// </summary>
    public class HtmlPropertyBuilder : IEnumerable<string>
    {
        private List<string> properties = new List<string>();
        /// <summary>
        /// 创建一个 <seealso cref="CssClassBuilder"/>
        /// </summary>
        /// <returns></returns>
        public static HtmlPropertyBuilder CreateCssClassBuilder()
        {
            return new CssClassBuilder();
        }
        /// <summary>
        /// 创建一个 <seealso cref="CssStyleBuilder"/>
        /// </summary>
        /// <returns></returns>
        public static HtmlPropertyBuilder CreateCssStyleBuilder()
        {
            return new CssStyleBuilder();
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

        /// <summary>
        /// 移除一个属性
        /// </summary>
        /// <param name="styleName"></param>
        /// <returns></returns>
        public HtmlPropertyBuilder Remove(string styleName)
        {
            var stylePrefix = $"{styleName}:";
            properties.RemoveAll(x => x.StartsWith(stylePrefix, StringComparison.CurrentCultureIgnoreCase));
            return this;
        }

        internal string ToString(string sep)
        {
            return string.Join(sep, properties.Where(x => !string.IsNullOrWhiteSpace(x)));
        }

        public override string ToString()
        {
            return ToString("");
        }

        public IEnumerator<string> GetEnumerator()
        {
            return new PropertyBuilderEnumerator(properties);
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new PropertyBuilderEnumerator(properties);
        }

        internal class PropertyBuilderEnumerator : IEnumerator<string>
        {
            private readonly List<string> properties;
            private string current;
            private int index = -1;
            public PropertyBuilderEnumerator(List<string> properties)
            {
                this.properties = properties;
            }
            public string Current => current;

            object IEnumerator.Current => current;

            public void Dispose()
            {

            }

            public bool MoveNext()
            {
                index++;
                if (properties.Count <= index)
                {
                    return false;
                }
                current = properties[index];
                return true;
            }

            public void Reset()
            {
                index = -1;
                current = null;
            }
        }
    }
}

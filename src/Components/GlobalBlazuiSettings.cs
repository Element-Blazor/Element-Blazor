using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    /// <summary>
    /// 全局配置
    /// </summary>
    public static class GlobalBlazuiSettings
    {
        /// <summary>
        /// 禁用所有组件动画，注：1、CSS 动画无法禁止，禁用动画的出发点是为了提高性能，CSS动画并不耗费性能，2、Toast（Message） 因支持同时弹出多个，所以不支持禁止
        /// </summary>
        public static bool DisableAnimation { get; set; }
    }
}

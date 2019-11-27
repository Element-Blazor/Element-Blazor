using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Form
{
    public class Activity
    {
        public string Name { get; set; }
        public string Area { get; set; }
        public DateTime? Time { get; set; }
        public bool Delivery { get; set; }
        public List<string> Type { get; set; }
        public string Resource { get; set; }
        public string Description { get; set; }

        public override string ToString()
        {
            return $"名称：{Name},区域：{Area},日期：{Time?.ToString()}，即时配送：{Delivery}，性质：{string.Join(",", Type)}，特殊资源：{Resource}，活动形式：{Description}";
        }
    }
}

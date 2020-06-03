


using Blazui.Component;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ServerRender.Demo.Form
{
    public class AutoGenerateFieldsActvity
    {
        [FormControl(Label = "名称")]
        public string Name { get; set; }
        [FormControl(Label = "区域")]
        public Area Area { get; set; }
        [FormControl(Label = "日期")]
        public DateTime? Time { get; set; }
        [FormControl(ControlType = typeof(BSwitch<bool>), Label = "即时配送")]
        public bool Delivery { get; set; }
        [FormControl(Label = "性质")]
        public List<string> Type { get; set; }
        [FormControl(Label = "特殊资源")]
        public string Resource { get; set; }
        [FormControl(Label = "活动形式")]
        public string Description { get; set; }

        public override string ToString()
        {
            return $"名称：{Name},区域：{Area},日期：{Time?.ToString()}，即时配送：{Delivery}，性质：{string.Join(",", Type)}，特殊资源：{Resource}，活动形式：{Description}";
        }
    }
}

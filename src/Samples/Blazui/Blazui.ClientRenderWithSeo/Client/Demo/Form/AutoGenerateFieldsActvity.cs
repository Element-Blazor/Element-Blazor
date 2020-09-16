


using Blazui.Component;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.ClientRenderWithSeo.Client.Demo.Form
{
    public class AutoGenerateFieldsActvity
    {
        [Display(Name = "名称")]
        public string Name { get; set; }
        [Display(Name = "区域")]
        public Area Area { get; set; }
        [Display(Name = "日期")]
        public DateTime? Time { get; set; }
        [Display(Name = "即时配送")]
        [FormControl(typeof(BSwitch<bool>))]
        public bool Delivery { get; set; }
        [Display(Name = "性质")]
        public List<string> Type { get; set; }
        [Display(Name = "特殊资源")]
        public string Resource { get; set; }
        [Display(Name = "活动形式")]
        public string Description { get; set; }

        public override string ToString()
        {
            return $"名称：{Name},区域：{Area},日期：{Time?.ToString()}，即时配送：{Delivery}，性质：{string.Join(",", Type)}，特殊资源：{Resource}，活动形式：{Description}";
        }
    }
}

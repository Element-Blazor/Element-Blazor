using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElLoading
    {
        [Parameter]
        public string IconClass { get; set; }
        [Parameter]
        public string Text { get; set; } = "∆¥√¸º”‘ÿ÷–";
        [Parameter]
        public string Background { get; set; }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components;

namespace Blazui.Component.Container
{
    public class TabOption
    {
        public string Name { get; set; }
        public bool? IsClosable { get; set; }
        public bool? IsDisabled { get; set; }
        public string Title { get; set; }
        public object Content { get; set; }
    }
}

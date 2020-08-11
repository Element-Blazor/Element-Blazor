using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace Blazui.Component.DisplayRenders
{
    public interface IDisplayRender
    {
        Func<object,object> CreateRender(TableHeader tableHeader);
    }
}

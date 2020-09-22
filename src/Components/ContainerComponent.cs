using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public interface IContainerComponent
    {
        LoadingService LoadingService { get; set; }
        ElementReference Container { get; set; }

    }
}

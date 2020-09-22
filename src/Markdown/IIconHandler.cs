using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element.Markdown
{
    public interface IIconHandler
    {
        Task HandleAsync(BMarkdownEditor editor);
    }
}

using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class MessageBoxContent
    {
        [Parameter]
        public MessageBoxOption Option { get; set; }
    }
}

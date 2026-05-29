using Microsoft.AspNetCore.Components.Web;

namespace Element
{
    public class AnchorClickEventArgs
    {
        public string Href { get; set; }

        public string Title { get; set; }

        public MouseEventArgs MouseEventArgs { get; set; }
    }
}

using System.Globalization;

namespace Element
{
    public class ElementConfig
    {
        public const string DefaultNamespace = "el";
        public const int DefaultZIndex = 2000;

        public ElementSize Size { get; set; } = ElementSize.Default;

        public string Namespace { get; set; } = DefaultNamespace;

        public int ZIndex { get; set; } = DefaultZIndex;

        public string Locale { get; set; } = CultureInfo.CurrentUICulture.Name;

        public ElementConfig Clone()
        {
            return new ElementConfig
            {
                Size = Size,
                Namespace = Namespace,
                ZIndex = ZIndex,
                Locale = Locale
            };
        }
    }
}

using Microsoft.AspNetCore.Components;
using System;

namespace Element
{
    public partial class ElConfigProvider : ElementComponentBase
    {
        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public ElementConfig Config { get; set; }

        [Parameter]
        public ElementSize? Size { get; set; }

        [Parameter]
        public string Namespace { get; set; }

        [Parameter]
        public int? ZIndex { get; set; }

        [Parameter]
        public string Locale { get; set; }

        protected ElementConfig ResolvedConfig { get; private set; } = new ElementConfig();

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            var config = ElementConfiguration?.Clone() ?? new ElementConfig();
            if (Config != null)
            {
                config = Merge(config, Config);
            }
            if (Size.HasValue)
            {
                config.Size = Size.Value;
            }
            if (!string.IsNullOrWhiteSpace(Namespace))
            {
                config.Namespace = Namespace.Trim();
            }
            if (ZIndex.HasValue)
            {
                config.ZIndex = ZIndex.Value;
            }
            if (!string.IsNullOrWhiteSpace(Locale))
            {
                config.Locale = Locale.Trim();
            }
            ResolvedConfig = config;
        }

        private static ElementConfig Merge(ElementConfig inheritedConfig, ElementConfig config)
        {
            if (config.Size != ElementSize.Default)
            {
                inheritedConfig.Size = config.Size;
            }
            if (!string.IsNullOrWhiteSpace(config.Namespace))
            {
                inheritedConfig.Namespace = config.Namespace.Trim();
            }
            if (config.ZIndex != ElementConfig.DefaultZIndex)
            {
                inheritedConfig.ZIndex = config.ZIndex;
            }
            if (!string.IsNullOrWhiteSpace(config.Locale))
            {
                inheritedConfig.Locale = config.Locale.Trim();
            }
            return inheritedConfig;
        }
    }
}

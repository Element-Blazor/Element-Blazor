using Bunit;
using Element;
using Microsoft.Extensions.DependencyInjection;
using System;
using Xunit;

namespace Element.ComponentTests
{
    public class ElWatermarkTests : BunitContext
    {
        public ElWatermarkTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void WatermarkRendersTextPatternOverContainer()
        {
            var cut = Render<ElWatermark>(parameters => parameters
                .Add(x => x.Content, "Confidential")
                .Add(x => x.Width, 160)
                .Add(x => x.Height, 80)
                .Add(x => x.Rotate, -18)
                .Add(x => x.ZIndex, 12)
                .Add(x => x.Gap, new[] { 40, 20 })
                .Add(x => x.Offset, new[] { 6, 8 })
                .Add(x => x.Font, new WatermarkFont
                {
                    Color = "rgba(64,158,255,.24)",
                    FontSize = 18,
                    FontWeight = "600"
                })
                .AddChildContent("<div class=\"document\">Quarterly report</div>"));

            var root = cut.Find(".el-watermark");
            var mask = cut.Find(".el-watermark__mask");
            var style = mask.GetAttribute("style");
            var decoded = Uri.UnescapeDataString(style);

            Assert.Contains("Quarterly report", root.TextContent);
            Assert.Contains("z-index:12", style);
            Assert.Contains("background-size:200px 100px", style);
            Assert.Contains("background-position:6px 8px", style);
            Assert.Contains("Confidential", decoded);
            Assert.Contains("rotate(-18)", decoded);
            Assert.Contains("font-size=\"18px\"", decoded);
            Assert.Contains("font-weight=\"600\"", decoded);
        }

        [Fact]
        public void WatermarkSupportsMultipleLinesAndImage()
        {
            var basic = Render<ElWatermark>();
            var decodedBasic = Uri.UnescapeDataString(basic.Find(".el-watermark__mask").GetAttribute("style"));

            Assert.Contains("Element Plus", decodedBasic);

            var text = Render<ElWatermark>(parameters => parameters
                .Add(x => x.ContentList, new[] { "Element", "Blazor" })
                .Add(x => x.Font, new WatermarkFont { TextAlign = "left", FontSize = "14px" }));

            var decodedText = Uri.UnescapeDataString(text.Find(".el-watermark__mask").GetAttribute("style"));

            Assert.Contains("Element", decodedText);
            Assert.Contains("Blazor", decodedText);
            Assert.Contains("text-anchor=\"start\"", decodedText);

            var image = Render<ElWatermark>(parameters => parameters
                .Add(x => x.Image, "data:image/png;base64,abc")
                .Add(x => x.Width, 96)
                .Add(x => x.Height, 48));

            var decodedImage = Uri.UnescapeDataString(image.Find(".el-watermark__mask").GetAttribute("style"));

            Assert.Contains("<image", decodedImage);
            Assert.Contains("data:image/png;base64,abc", decodedImage);
            Assert.Contains("width=\"96\"", decodedImage);
            Assert.Contains("height=\"48\"", decodedImage);
        }
    }
}

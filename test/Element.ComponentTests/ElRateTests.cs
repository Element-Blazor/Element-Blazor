using Bunit;
using Element;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Element.ComponentTests
{
    public class ElRateTests : BunitContext
    {
        public ElRateTests()
        {
            Services.AddElementServices();
        }

        [Fact]
        public void RendersCustomIconTemplate()
        {
            RenderFragment<RateIconContext> template = context => builder =>
            {
                builder.OpenElement(0, "span");
                builder.AddAttribute(1, "class", context.Active ? "custom-rate active" : "custom-rate");
                builder.AddContent(2, context.Index);
                builder.CloseElement();
            };

            var cut = Render<ElRate>(parameters => parameters
                .Add(x => x.Value, 2)
                .Add(x => x.IconTemplate, template));

            Assert.Equal(5, cut.FindAll(".custom-rate").Count);
            Assert.Equal(2, cut.FindAll(".custom-rate.active").Count);
        }
    }
}

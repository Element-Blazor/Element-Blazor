using Microsoft.AspNetCore.Components;
using System.Linq;

namespace Element
{
    public partial class ElCarouselItem : ElementComponentBase
    {
        [CascadingParameter]
        public ElCarousel Carousel { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int Order { get; set; }

        internal bool IsActive => Carousel?.Items.ToList().IndexOf(this) == Carousel?.ActiveIndex;

        protected string ItemClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-carousel__item", Cls)
            .AddIf(IsActive, "is-active")
            .ToString();

        protected string ItemStyle => HtmlPropertyBuilder.CreateCssStyleBuilder()
            .AddIf(!IsActive, "display:none")
            .Add(Style)
            .ToString();

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Carousel?.AddItem(this);
        }

        public override void Dispose()
        {
            Carousel?.RemoveItem(this);
            base.Dispose();
        }
    }
}

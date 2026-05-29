using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;

namespace Element
{
    public partial class ElDescriptions : ElementComponentBase
    {
        private readonly List<ElDescriptionsItem> items = new List<ElDescriptionsItem>();

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public RenderFragment Extra { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public int Column { get; set; } = 3;

        [Parameter]
        public bool Border { get; set; }

        [Parameter]
        public DescriptionsDirection Direction { get; set; } = DescriptionsDirection.Horizontal;

        [Parameter]
        public ElementSize Size { get; set; } = ElementSize.Default;

        internal IReadOnlyList<ElDescriptionsItem> Items => items.OrderBy(x => x.Order).ToList();

        internal int EffectiveColumn => Column <= 0 ? 1 : Column;

        internal void AddItem(ElDescriptionsItem item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                Refresh();
            }
        }

        internal void RemoveItem(ElDescriptionsItem item)
        {
            if (items.Remove(item))
            {
                Refresh();
            }
        }

        protected string DescriptionsClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-descriptions", Cls)
            .AddIf(Border, "is-bordered")
            .AddIf(Size == ElementSize.Large, "el-descriptions--large")
            .AddIf(Size == ElementSize.Small, "el-descriptions--small")
            .ToString();

        protected string ContentStyle => $"grid-template-columns:repeat({EffectiveColumn}, minmax(0, 1fr))";

        protected IReadOnlyList<IReadOnlyList<ElDescriptionsItem>> Rows
        {
            get
            {
                var rows = new List<IReadOnlyList<ElDescriptionsItem>>();
                var row = new List<ElDescriptionsItem>();
                var span = 0;

                foreach (var item in Items)
                {
                    var itemSpan = item.NormalizedSpan;
                    if (row.Count > 0 && span + itemSpan > EffectiveColumn)
                    {
                        rows.Add(row);
                        row = new List<ElDescriptionsItem>();
                        span = 0;
                    }

                    row.Add(item);
                    span += itemSpan;
                }

                if (row.Count > 0)
                {
                    rows.Add(row);
                }

                return rows;
            }
        }
    }
}

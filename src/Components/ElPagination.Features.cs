using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElPagination : ElementComponentBase
    {
        [Parameter]
        public string Layout { get; set; } = "prev, pager, next";

        [Parameter]
        public int[] PageSizes { get; set; } = { 10, 20, 50, 100 };

        [Parameter]
        public EventCallback<int> PageSizeChanged { get; set; }

        protected IReadOnlyList<PaginationLayoutPart> LayoutParts => (Layout ?? "prev, pager, next")
            .Split(',', StringSplitOptions.RemoveEmptyEntries)
            .Select(x => x.Trim().ToLowerInvariant())
            .Select(x => x switch
            {
                "prev" => PaginationLayoutPart.Prev,
                "pager" => PaginationLayoutPart.Pager,
                "next" => PaginationLayoutPart.Next,
                "jumper" => PaginationLayoutPart.Jumper,
                "total" => PaginationLayoutPart.Total,
                "sizes" => PaginationLayoutPart.Sizes,
                _ => (PaginationLayoutPart?)null
            })
            .Where(x => x.HasValue)
            .Select(x => x.Value)
            .ToList();

        protected async Task OnJumperChangeAsync(ChangeEventArgs e)
        {
            if (int.TryParse(Convert.ToString(e.Value, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var page))
            {
                Jump(page);
            }

            await Task.CompletedTask;
        }

        protected async Task OnPageSizeChangeAsync(ChangeEventArgs e)
        {
            if (!int.TryParse(Convert.ToString(e.Value, CultureInfo.InvariantCulture), NumberStyles.Integer, CultureInfo.InvariantCulture, out var size))
            {
                return;
            }

            PageSize = size;
            CurrentPage = 1;
            pageCount = Convert.ToInt32(Math.Ceiling((float)Total / Math.Max(1, PageSize)));
            SwitchButtonStatus();
            if (PageSizeChanged.HasDelegate)
            {
                await PageSizeChanged.InvokeAsync(size);
            }

            if (CurrentPageChanged != null)
            {
                await CurrentPageChanged(CurrentPage);
            }
        }
    }
}

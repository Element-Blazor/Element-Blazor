using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElXPrompts : ElementComponentBase
    {
        [Parameter]
        public IEnumerable<XPromptItem> Items { get; set; } = Enumerable.Empty<XPromptItem>();

        [Parameter]
        public EventCallback<XPromptItem> OnSelect { get; set; }

        [Parameter]
        public bool Vertical { get; set; }

        protected string PromptsClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-prompts", Cls)
            .AddIf(Vertical, "is-vertical")
            .ToString();

        protected string GetItemClass(XPromptItem item) => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-x-prompts__item")
            .AddIf(item?.Disabled == true, "is-disabled")
            .ToString();

        private async Task SelectAsync(XPromptItem item)
        {
            if (item == null || item.Disabled || !OnSelect.HasDelegate)
            {
                return;
            }

            await OnSelect.InvokeAsync(item);
        }
    }
}

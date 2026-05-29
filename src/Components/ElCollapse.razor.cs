using Microsoft.AspNetCore.Components;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElCollapse : ElementComponentBase
    {
        private readonly List<ElCollapseItem> items = new List<ElCollapseItem>();
        private IList<string> activeNames = new List<string>();

        [Parameter]
        public string Value { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }

        [Parameter]
        public IList<string> Values { get; set; }

        [Parameter]
        public EventCallback<IList<string>> ValuesChanged { get; set; }

        [Parameter]
        public bool Accordion { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        internal bool IsActive(string name)
        {
            return activeNames.Contains(name);
        }

        internal void AddItem(ElCollapseItem item)
        {
            if (!items.Contains(item))
            {
                items.Add(item);
                Refresh();
            }
        }

        internal void RemoveItem(ElCollapseItem item)
        {
            if (items.Remove(item))
            {
                Refresh();
            }
        }

        internal async Task ToggleItemAsync(ElCollapseItem item)
        {
            if (item.Disabled)
            {
                return;
            }

            var name = item.EffectiveName;
            var next = activeNames.ToList();
            if (Accordion)
            {
                next = next.Contains(name) ? new List<string>() : new List<string> { name };
            }
            else if (next.Contains(name))
            {
                next.Remove(name);
            }
            else
            {
                next.Add(name);
            }

            activeNames = next;
            Values = next;
            Value = next.FirstOrDefault();
            if (ValuesChanged.HasDelegate)
            {
                await ValuesChanged.InvokeAsync(next);
            }

            if (ValueChanged.HasDelegate)
            {
                await ValueChanged.InvokeAsync(Value);
            }

            Refresh();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (Values != null)
            {
                activeNames = Values.Where(x => !string.IsNullOrWhiteSpace(x)).ToList();
            }
            else if (!string.IsNullOrWhiteSpace(Value))
            {
                activeNames = new List<string> { Value };
            }
            else
            {
                activeNames = new List<string>();
            }
        }

        protected string CollapseClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-collapse", Cls)
            .ToString();
    }
}

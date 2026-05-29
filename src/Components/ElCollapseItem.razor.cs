using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using System.Threading.Tasks;

namespace Element
{
    public partial class ElCollapseItem : ElementComponentBase
    {
        private static int nextId;

        [CascadingParameter]
        public ElCollapse Collapse { get; set; }

        [Parameter]
        public string Name { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public RenderFragment ChildContent { get; set; }

        [Parameter]
        public bool Disabled { get; set; }

        internal string EffectiveName { get; private set; }

        protected bool IsActive => Collapse?.IsActive(EffectiveName) == true;

        protected string IsActiveText => IsActive.ToString().ToLower();

        protected string ItemClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-collapse-item", Cls)
            .AddIf(IsActive, "is-active")
            .AddIf(Disabled, "is-disabled")
            .ToString();

        protected string HeaderClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-collapse-item__header")
            .AddIf(IsActive, "is-active")
            .AddIf(Disabled, "is-disabled")
            .ToString();

        protected string WrapStyle => IsActive ? string.Empty : "display:none";

        protected override void OnInitialized()
        {
            base.OnInitialized();
            EffectiveName = string.IsNullOrWhiteSpace(Name) ? $"collapse-item-{++nextId}" : Name;
            Collapse?.AddItem(this);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!string.IsNullOrWhiteSpace(Name))
            {
                EffectiveName = Name;
            }
        }

        protected async Task ToggleAsync()
        {
            if (Collapse != null)
            {
                await Collapse.ToggleItemAsync(this);
            }
        }

        protected async Task OnKeyDownAsync(KeyboardEventArgs e)
        {
            if (e.Key == "Enter" || e.Key == " ")
            {
                await ToggleAsync();
            }
        }

        public override void Dispose()
        {
            Collapse?.RemoveItem(this);
            base.Dispose();
        }
    }
}

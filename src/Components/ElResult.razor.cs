using Microsoft.AspNetCore.Components;

namespace Element
{
    public partial class ElResult : ElementComponentBase
    {
        [Parameter]
        public ResultIcon Icon { get; set; } = ResultIcon.Info;

        [Parameter]
        public RenderFragment IconContent { get; set; }

        [Parameter]
        public string Title { get; set; }

        [Parameter]
        public RenderFragment TitleContent { get; set; }

        [Parameter]
        public string SubTitle { get; set; }

        [Parameter]
        public RenderFragment SubTitleContent { get; set; }

        [Parameter]
        public RenderFragment Extra { get; set; }

        protected string ResultClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-result", Cls)
            .ToString();

        protected string IconClass => HtmlPropertyBuilder.CreateCssClassBuilder()
            .Add("el-result-icon", $"el-result-icon--{Icon.ToString().ToLower()}", Icon switch
            {
                ResultIcon.Success => "el-icon-success",
                ResultIcon.Warning => "el-icon-warning",
                ResultIcon.Error => "el-icon-error",
                _ => "el-icon-info"
            })
            .ToString();
    }
}

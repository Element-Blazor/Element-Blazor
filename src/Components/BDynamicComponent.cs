using Blazui.Component.ControlConfigs;
using Blazui.Component.ControlRender;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using Microsoft.AspNetCore.Components.RenderTree;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Blazui.Component
{
    /// <summary>
    /// 提供动态加载组件的功能
    /// </summary>
    public class BDynamicComponent : ComponentBase
    {
        [Inject]
        private IServiceProvider provider { get; set; }
        [Parameter]
        public object Component { get; set; }

        protected override async Task OnParametersSetAsync()
        {
            if (Config?.DataSourceLoader == null)
            {
                return;
            }
            var loader = (IDataSourceLoader)provider.GetRequiredService(Config.DataSourceLoader);
            Config.DataSource = await loader.LoadAsync();
            StateHasChanged();
        }
        /// <summary>
        /// 渲染配置
        /// </summary>
        [Parameter]
        public RenderConfig Config { get; set; }

        [Parameter]
        public IDictionary<string, object> Parameters { get; set; } = new Dictionary<string, object>();

        protected override void BuildRenderTree(RenderTreeBuilder builder)
        {
            var render = Component as IControlRender;
            if (render != null)
            {
                if (Config == null)
                {
                    throw new BlazuiException("Component 类型为 IControlRender 时，必须指定 Config 参数");
                }

                render.Render(builder, Config);
                return;
            }
            else if (Component is Type type)
            {
                builder.OpenComponent(0, type);
                var seq = 1;
                foreach (var key in Parameters.Keys)
                {
                    builder.AddAttribute(seq++, key, Parameters[key]);
                }
                builder.CloseComponent();
                return;
            }
            if (Component is RenderFragment renderFragment)
            {
                builder.AddContent(0, renderFragment);
                return;
            }
            builder.AddMarkupContent(0, Convert.ToString(Component));
        }
    }
}

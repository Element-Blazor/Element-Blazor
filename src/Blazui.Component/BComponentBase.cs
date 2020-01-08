using Blazui.Component.Diagnose;
using Blazui.Component.Form;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public class BComponentBase : ComponentBase
    {
        protected bool RequireRender { get; set; }
        internal IDictionary<string, List<DiagnoseModel>> performanceInfos = new Dictionary<string, List<DiagnoseModel>>();
        [Inject]
        MessageBox MessageBox { get; set; }

        [Inject]
        public DialogService DialogService { get; set; }

        [Inject]
        public NavigationManager NavigationManager { get; set; }
        [Inject]
        public IJSRuntime JSRuntime { get; set; }

        [Inject]
        MessageService MessageService { get; set; }

        [Inject]
        public LoadingService LoadingService { get; set; }


        [Parameter]
        public Func<object, Task> OnRenderCompleted { get; set; }

        [CascadingParameter]
        public BBadgeBase Badge { get; set; }
        /// <summary>
        /// 设置自定义样式
        /// </summary>
        [Parameter]
        public string Style { get; set; } = string.Empty;

        /// <summary>
        /// 启用性能诊断
        /// </summary>
        [Parameter]
        public bool EnablePerformanceDiagnose { get; set; }

        /// <summary>
        /// 弹出 Alert 消息
        /// </summary>
        /// <param name="text"></param>
        public void Alert(string text)
        {
            _ = MessageBox.AlertAsync(text);
        }
        /// <summary>
        /// 弹出 Confirm 消息
        /// </summary>
        /// <param name="text"></param>
        public async Task<MessageBoxResult> ConfirmAsync(string text)
        {
            return await MessageBox.ConfirmAsync(text);
        }

        /// <summary>
        /// 弹出 Confirm 消息
        /// </summary>
        /// <param name="text"></param>
        public async Task<MessageBoxResult> ConfirmAsync(string text)
        {
            return await MessageBox.ConfirmAsync(text);
        }

        /// <summary>
        /// 默认情况下所有复杂组件都只进行一次渲染，该方法将组件置为需要再次渲染
        /// </summary>
        public void MarkAsRequireRender()
        {
            RequireRender = true;
        }

        public void Toast(string text)
        {
            MessageService.Show(text);
        }
        public async Task<MessageBoxResult> AlertAsync(string text)
        {
            return await MessageBox.AlertAsync(text);
        }

        protected override void OnInitialized()
        {
            if (!EnablePerformanceDiagnose)
            {
                return;
            }
            var stacktrace = new StackTrace();
            AddDiagnose(nameof(OnInitialized), stacktrace);
        }

        protected override async Task OnInitializedAsync()
        {
            if (!EnablePerformanceDiagnose)
            {
                await Task.CompletedTask;
                return;
            }
            var stacktrace = new StackTrace();
            AddDiagnose(nameof(OnInitializedAsync), stacktrace);
        }

        protected override void OnAfterRender(bool firstRender)
        {
            RequireRender = false;
            if (!EnablePerformanceDiagnose)
            {
                return;
            }
            var stacktrace = new StackTrace();
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(firstRender), firstRender);
            AddDiagnose(nameof(OnAfterRender), stacktrace, parameters);
            Badge.SetValue(performanceInfos.Count);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (!firstRender)
            {
                return;
            }
            if (OnRenderCompleted != null)
            {
                await OnRenderCompleted(this);
            }
            RequireRender = false;
            if (!EnablePerformanceDiagnose)
            {
                await Task.CompletedTask;
                return;
            }
            var stacktrace = new StackTrace();
            var parameters = new Dictionary<string, object>();
            parameters.Add(nameof(firstRender), firstRender);
            AddDiagnose(nameof(OnAfterRenderAsync), stacktrace, parameters);
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (!EnablePerformanceDiagnose)
            {
                return;
            }
            var stacktrace = new StackTrace();
            AddDiagnose(nameof(OnParametersSet), stacktrace);
        }

        protected override async Task OnParametersSetAsync()
        {
            if (!EnablePerformanceDiagnose)
            {
                await Task.CompletedTask;
                return;
            }
            var stacktrace = new StackTrace();
            AddDiagnose(nameof(OnParametersSetAsync), stacktrace);
        }

        public override async Task SetParametersAsync(ParameterView parameters)
        {
            await base.SetParametersAsync(parameters);
            if (!EnablePerformanceDiagnose)
            {
                await Task.CompletedTask;
                return;
            }
            var stacktrace = new StackTrace();
            AddDiagnose(nameof(SetParametersAsync), stacktrace, parameters.ToDictionary());
        }

        void AddDiagnose(string methodName, StackTrace stackTrace, IReadOnlyDictionary<string, object> parameters = null)
        {
            var diagnoseModel = new DiagnoseModel()
            {
                MethodName = methodName,
                StackTrace = stackTrace.ToString(),
                Parameters = parameters
            };
            performanceInfos.TryGetValue(methodName, out var diagnoseInfos);
            if (diagnoseInfos == null)
            {
                diagnoseInfos = new List<DiagnoseModel>();
                performanceInfos.Add(methodName, diagnoseInfos);
            }
            diagnoseInfos.Add(diagnoseModel);
        }

        public void Refresh()
        {
            StateHasChanged();
        }

        protected override bool ShouldRender()
        {
            return RequireRender;
        }
    }
}

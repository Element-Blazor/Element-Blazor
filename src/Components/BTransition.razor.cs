using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Element
{
    public partial class BTransition : BComponentBase
    {
        private ElementReference animationElement;
        private bool animationBegined = false;
        private HtmlPropertyBuilder styleBuilder;
        private List<string> currentStyleNames;
        private Queue<TransitionOption> paths = new Queue<TransitionOption>();
        private DotNetObjectReference<BTransition> thisRef;
        private TransitionOption currentPath;
        private HtmlPropertyBuilder clsBuilder;
        private TaskCompletionSource<int> taskTrigger;
        private bool animating = false;

        /// <summary>
        /// 当动画暂停时触发
        /// </summary>
        [Parameter]
        public EventCallback OnPause { get; set; }

        /// <summary>
        /// 根据动画元素尺寸自动调整位置，一般用于弹窗位置调整
        /// </summary>
        [Parameter]
        public bool AutoAdjustPosition { get; set; }

        /// <summary>
        /// 当所有动画执行完成后触发
        /// </summary>
        [Parameter]
        public EventCallback OnAllEnd { get; set; }

        /// <summary>
        /// 绝对定位
        /// </summary>
        [Parameter]
        public bool IsAbsolute { get; set; }

        [Inject]
        internal Document Document { get; set; }

        /// <summary>
        /// 动画执行路径
        /// </summary>
        [Parameter]
        public RenderFragment Path { get; set; }

        /// <summary>
        /// 动画开始之前的样式
        /// </summary>
        [Parameter]
        public string InitialStyle { get; set; }

        internal void AddPathConfig(TransitionOption option)
        {
            if (!IsAbsolute && (option.Style.StartsWith("top:") || option.Style.StartsWith("left:") || option.Style.Contains(";top:") || option.Style.Contains(";left:")))
            {
                IsAbsolute = true;
            }
            paths.Enqueue(option);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            styleBuilder = HtmlPropertyBuilder.CreateCssStyleBuilder()
                .Add("display:none");
        }

        /// <summary>
        /// 要进行动画播放的内容
        /// </summary>
        [Parameter]
        public RenderFragment Content { get; set; }

        private async Task AnimationElementLoadAsync()
        {
            if (paths == null || !paths.Any())
            {
                return;
            }
            if (AutoAdjustPosition)
            {
                var rect = await animationElement.Dom(JSRuntime).GetBoundingClientRectAsync();
                var screenWidth = await Document.GetClientWidthAsync();
                var left = screenWidth / 2 - rect.Width / 2;
                styleBuilder.Add($"left:{left}px");
            }
            currentPath = paths.Dequeue();
            await Task.Delay(currentPath.Delay);
            ExecuteAnimation(currentPath.Style, currentPath.Increment);
        }

        private void ExecuteAnimation(string style, bool increment)
        {
            animating = true;
            var styles = style.Split(';');
            currentStyleNames = styles.Select(x => x.Split(':')[0]).ToList();
            for (int i = 0; i < currentStyleNames.Count; i++)
            {
                var styleName = currentStyleNames[i];
                if (styleName == "margin")
                {
                    currentStyleNames[i] = "margin-left";
                    currentStyleNames.AddRange(new string[] { "margin-right", "margin-bottom", "margin-top" });
                    continue;
                }
                if (styleName == "padding")
                {
                    currentStyleNames[i] = "padding-left";
                    currentStyleNames.AddRange(new string[] { "padding-right", "padding-bottom", "padding-top" });
                    continue;
                }
            }
            foreach (var styleItem in styles)
            {
                var styleName = styleItem.Split(':')[0];
                var finalStyle = styleItem;
                if (increment && styleItem.EndsWith("px", StringComparison.CurrentCultureIgnoreCase))
                {
                    var incrementValue = Convert.ToDouble(Regex.Match(styleItem, @"\d+\.*").Value);
                    var oldStyle = styleBuilder.FirstOrDefault(x => x.StartsWith($"{styleName}:"));
                    if (oldStyle == null)
                    {
                        styleBuilder.Remove(styleName).Add(finalStyle);
                        continue;
                    }
                    var oldValue = Convert.ToDouble(Regex.Match(oldStyle, @"\d+\.*").Value);
                    finalStyle = $"{styleName}:{(oldValue + incrementValue).ToString()}px";
                    styleBuilder.Remove(styleName).Add(finalStyle);
                    continue;
                }
                styleBuilder.Remove(styleName).Add(finalStyle);
            }
            MarkAsRequireRender();
            StateHasChanged();
        }

        /// <summary>
        /// 使暂停的动画继续执行
        /// </summary>
        public void Resume()
        {
            Console.WriteLine("resume:" + this.GetHashCode());
            if (taskTrigger == null)
            {
                return;
            }
            taskTrigger.TrySetResult(0);
        }

        [JSInvokable("AnimationEnd")]
        public async Task AnimationEndAsync(string propertyName)
        {
            if (currentStyleNames != null)
            {
                if (!currentStyleNames.Remove(propertyName))
                {
                    Console.WriteLine("样式移除失败:" + propertyName + "，还有" + currentStyleNames.Count + "个样式等待移除");
                }
                else
                {
                    Console.WriteLine($"{propertyName}移除成功，还有{currentStyleNames.Count  }个样式等待移除");
                    while (currentStyleNames.Contains(propertyName))
                    {
                        currentStyleNames.Remove(propertyName);
                    }
                    if (currentStyleNames.Any())
                    {
                        return;
                    }
                }
            }

            if (!animating)
            {
                return;
            }
            animating = false;
            if (paths == null || !paths.Any())
            {
                if (OnAllEnd.HasDelegate)
                {
                    await OnAllEnd.InvokeAsync(null);
                }
                return;
            }
            currentPath = paths.Dequeue();
            if (currentPath.Pause.HasValue && currentPath.Pause.Value)
            {
                if (OnPause.HasDelegate)
                {
                    await OnPause.InvokeAsync(null);
                }
                taskTrigger = new TaskCompletionSource<int>();
                await taskTrigger.Task;
                taskTrigger = null;
            }
            await Task.Delay(currentPath.Delay);
            styleBuilder.Remove("transition").Add($"transition:all {(currentPath.Duration <= 0 ? 1000 : currentPath.Duration)}ms ease");
            ExecuteAnimation(currentPath.Style, currentPath.Increment);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            await base.OnAfterRenderAsync(firstRender);
            if (firstRender)
            {
                thisRef = DotNetObjectReference.Create(this);
                clsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                    .Add("blazor-animation")
                    .Add(Cls?.Split(' ') ?? new string[0]);

                await JSRuntime.InvokeVoidAsync("RegisterAnimationBegin", thisRef, animationElement);
                styleBuilder = HtmlPropertyBuilder.CreateCssStyleBuilder()
                    .Add(InitialStyle?.Split(';') ?? new string[0])
                    .AddIf(IsAbsolute, "position:absolute")
                    .Add("transition:all 1s ease");
                if (paths.Any())
                {
                    var firstStyle = paths.Peek();
                    styleBuilder.Remove("transition").Add($"transition:all {(firstStyle.Duration <= 0 ? 1000 : firstStyle.Duration)}ms ease");
                }
                MarkAsRequireRender();
                StateHasChanged();
                return;
            }
            if (!animationBegined)
            {
                animationBegined = true;
                await AnimationElementLoadAsync();
            }
        }

        public override void Dispose()
        {
            thisRef?.Dispose();
        }
    }
}

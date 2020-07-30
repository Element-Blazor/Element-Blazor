using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Mvc.Internal;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Blazui.Component
{
    public partial class BTransition
    {
        private ElementReference animationElement;
        private bool animationBegined = false;
        private HtmlPropertyBuilder styleBuilder;
        private List<string> currentStyleNames;
        private Queue<(string Style, int Delay, bool Increment, int Duration)> paths = new Queue<(string Style, int Delay, bool Increment, int Duration)>();
        private DotNetObjectReference<BTransition> thisRef;
        private (string Style, int Delay, bool Increment, int Duration) currentPath;
        private HtmlPropertyBuilder clsBuilder;

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

        internal void AddPathConfig(string style, int delay, bool increment, int duration)
        {
            if (!IsAbsolute && (style.StartsWith("top:") || style.StartsWith("left:") || style.Contains(";top:") || style.Contains(";left:")))
            {
                IsAbsolute = true;
            }
            paths.Enqueue((style, delay, increment, duration));
        }

        protected override void OnInitialized()
        {
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
            currentPath = paths.Dequeue();
            await Task.Delay(currentPath.Delay);
            ExecuteAnimation(currentPath.Style, currentPath.Increment);
        }

        private void ExecuteAnimation(string style, bool increment)
        {
            var styles = style.Split(';');
            currentStyleNames = styles.Select(x => x.Split(':')[0]).ToList();
            Console.Write(style);
            Console.WriteLine(currentStyleNames.Count);
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

        [JSInvokable("AnimationEnd")]
        public async Task AnimationEndAsync(string propertyName)
        {
            if (currentStyleNames != null)
            {
                if (!currentStyleNames.Remove(propertyName))
                {
                    Console.WriteLine("样式移除失败:" + propertyName + "，还有" + currentStyleNames.Count + "个样式等待移除");
                    throw new BlazuiException("样式移除失败:" + propertyName + "，还有" + currentStyleNames.Count + "个样式等待移除");
                }
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

            if (paths == null || !paths.Any())
            {
                if (OnAllEnd.HasDelegate)
                {
                    await OnAllEnd.InvokeAsync(null);
                }
                return;
            }
            currentPath = paths.Dequeue();
            await Task.Delay(currentPath.Delay);
            styleBuilder.Remove("transition").Add($"transition:all {(currentPath.Duration <= 0 ? 1000 : currentPath.Duration)}ms ease");
            ExecuteAnimation(currentPath.Style, currentPath.Increment);
        }

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                thisRef = DotNetObjectReference.Create(this);
                clsBuilder = HtmlPropertyBuilder.CreateCssClassBuilder()
                    .Add("blazor-animation")
                    .AddIf(!string.IsNullOrWhiteSpace(Cls), Cls.Split(' '));

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

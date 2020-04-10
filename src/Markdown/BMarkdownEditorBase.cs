
using Blazui.Component;

using Markdig;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace Blazui.Markdown
{
    public class BMarkdownEditorBase : BFieldComponentBase<string>
    {
        internal static IDictionary<Icon, IconDescriptionAttribute> allIcons = new Dictionary<Icon, IconDescriptionAttribute>();

        internal MarkupString previewHtml = (MarkupString)string.Empty;
        internal IDictionary<Icon, IconDescriptionAttribute> icons;
        private bool editorRendered = false;
        private MarkdownPipeline pipeline = new MarkdownPipelineBuilder().UseAdvancedExtensions().Build();
        /// <summary>
        /// 值
        /// </summary>
        [Parameter]
        public string Value { get; set; }

        /// <summary>
        /// 图片上传窗口的提示
        /// </summary>
        [Parameter]
        public string ImageUploadTip { get; set; }

        /// <summary>
        /// 文件上传窗口的提示
        /// </summary>
        [Parameter]
        public string FileUploadTip { get; set; }

        [Inject]
        private IServiceProvider serviceProvider { get; set; }

        /// <summary>
        /// 当编辑器滚动时，预览跟着滚动
        /// </summary>
        [Parameter]
        public bool EnableSyncScroll { get; set; } = true;

        /// <summary>
        /// 禁用文件上传
        /// </summary>
        [Parameter]
        public bool DisableFileUpload { get; set; }

        /// <summary>
        /// 禁用图片上传
        /// </summary>
        [Parameter]
        public bool DisableImageUpload { get; set; }

        [Parameter]
        public EventCallback<string> ValueChanged { get; set; }
        /// <summary>
        /// 工具栏图标
        /// </summary>
        [Parameter]
        public Icon[] Icons { get; set; }

        /// <summary>
        /// 高度
        /// </summary>
        [Parameter]
        public float Height { get; set; } = 500;

        /// <summary>
        /// 文件上传地址
        /// </summary>
        [Parameter]
        public string UploadUrl { get; set; }

        /// <summary>
        /// 单文件图片最大限制，KB为单位
        /// </summary>
        [Parameter]
        public long ImageMaxSize { get; set; }

        /// <summary>
        /// 单文件最大限制，KB为单位
        /// </summary>
        [Parameter]
        public long FileMaxSize { get; set; }
        /// <summary>
        /// 图片最大宽度
        /// </summary>
        [Parameter]
        public float ImageWidth { get; set; }

        /// <summary>
        /// 图片最大高度
        /// </summary>
        [Parameter]
        public float ImageHeight { get; set; }

        /// <summary>
        /// 允许上传的图片文件后缀
        /// </summary>
        [Parameter]
        public string[] AllowImageExtensions { get; set; } = new string[] { ".jpg", ".png", ".jpeg", ".gif", ".bmp" };

        /// <summary>
        /// 允许上传的普通文件后缀
        /// </summary>
        [Parameter]
        public string[] AllowFileExtensions { get; set; } = new string[0];

        internal protected ElementReference textarea;

        internal ElementReference Textarea
        {
            get
            {
                return textarea;
            }
        }
        internal ElementReference preview;

        static BMarkdownEditorBase()
        {
            var iconType = typeof(Icon);
            var iconNames = Enum.GetNames(typeof(Icon));
            var handlers = Assembly.GetExecutingAssembly().GetExportedTypes()
                 .Where(x => x.IsClass)
                 .Where(x => x.Namespace.EndsWith("IconHandlers"))
                 .Where(x => x.GetInterfaces().Any(x => x == typeof(IIconHandler)))
                 .ToList();
            foreach (var iconName in iconNames)
            {
                var iconDescription = iconType.GetField(iconName).GetCustomAttributes(false).OfType<IconDescriptionAttribute>().FirstOrDefault() ?? new IconDescriptionAttribute();
                if (string.IsNullOrWhiteSpace(iconDescription.IconCls))
                {
                    iconDescription.IconCls = "fa fa-" + iconName.ToLower();
                }
                if (string.IsNullOrWhiteSpace(iconDescription.Title))
                {
                    iconDescription.Title = iconName;
                }
                if (iconDescription.Handler == null)
                {
                    iconDescription.Handler = handlers.FirstOrDefault(x => x.Name == $"{iconName}Handler");
                }
                allIcons.Add(Enum.Parse<Icon>(iconName), iconDescription);
            }
        }

        protected override void FormItem_OnReset(object value, bool requireRerender)
        {
            Value = value?.ToString();
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            StateHasChanged();
        }

        protected override void OnParametersSet()
        {
            base.OnParametersSet();
            if (FormItem == null)
            {
                return;
            }
            if (FormItem.OriginValueHasRendered)
            {
                SetFieldValue(Value ?? FormItem.Value, false);
                return;
            }
            FormItem.OriginValueHasRendered = true;
            if (FormItem.Form.Values.Any())
            {
                Value = FormItem.OriginValue;
            }
            SetFieldValue(Value, false);
        }

        internal protected void Handle(IconDescriptionAttribute iconDescription)
        {
            var handler = (IIconHandler)serviceProvider.GetService(iconDescription.Handler);
            if (handler == null)
            {
                Alert("该图标没有对应的处理程序");
                return;
            }
            handler.HandleAsync(this);
        }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            if (Icons == null)
            {
                icons = allIcons;
            }
            else
            {
                icons = allIcons.Where(x => Icons.Contains(x.Key)).ToDictionary(x => x.Key, x => x.Value);
            }
        }

        protected override void OnAfterRender(bool firstRender)
        {
            base.OnAfterRender(firstRender);
            if (editorRendered)
            {
                return;
            }
            editorRendered = true;
            JSRuntime.InvokeVoidAsync("initilizeEditor", textarea, DotNetObjectReference.Create(this), Height, Value ?? string.Empty, EnableSyncScroll);
            RefreshPreview(Value);
        }

        [JSInvokable("scrollPreview")]
        public void ScrollPreview(float scrollTop)
        {
            JSRuntime.InvokeVoidAsync("scrollPreview", preview, scrollTop);
        }

        [JSInvokable("refreshPreview")]
        public void RefreshPreview(string value)
        {
            Value = value?.Trim();
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            SetFieldValue(Value, true);
            previewHtml = (MarkupString)Markdig.Markdown.ToHtml(value ?? string.Empty, pipeline);
            RequireRender = true;
            StateHasChanged();
        }
    }
}

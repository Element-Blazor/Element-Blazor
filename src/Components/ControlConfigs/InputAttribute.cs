namespace Element.ControlConfigs
{
    public class InputAttribute : BaseAttribute
    {
        private bool clearable;
        private bool disabled;

        /// <summary>
        /// 输入框类型
        /// </summary>
        public InputType Type { get; set; } = InputType.Text;

        /// <summary>
        /// 输入框尺寸
        /// </summary>
        public InputSize Size { get; set; } = InputSize.Normal;

        /// <summary>
        /// Placeholder
        /// </summary>
        public string Placeholder { get; set; }

        /// <summary>
        /// 是否禁用输入框
        /// </summary>
        public bool Disabled
        {
            get => disabled;
            set => disabled = value;
        }

        /// <summary>
        /// 是否禁用输入框
        /// </summary>
        public bool IsDisabled
        {
            get => disabled;
            set => disabled = value;
        }

        /// <summary>
        /// 是否只读
        /// </summary>
        public bool Readonly { get; set; }

        /// <summary>
        /// 是否可清空
        /// </summary>
        public bool Clearable
        {
            get => clearable;
            set => clearable = value;
        }

        /// <summary>
        /// 是否可清空
        /// </summary>
        public bool IsClearable
        {
            get => clearable;
            set => clearable = value;
        }

        /// <summary>
        /// 前缀图标
        /// </summary>
        public string PrefixIcon { get; set; }

        /// <summary>
        /// 后缀图标
        /// </summary>
        public string SuffixIcon { get; set; }

        /// <summary>
        /// 最大输入长度，小于 0 时不设置。
        /// </summary>
        public int Maxlength { get; set; } = -1;

        /// <summary>
        /// 最小输入长度，小于 0 时不设置。
        /// </summary>
        public int Minlength { get; set; } = -1;

        /// <summary>
        /// Textarea 缩放策略。
        /// </summary>
        public string Resize { get; set; }

        /// <summary>
        /// Textarea 是否自适应高度。
        /// </summary>
        public bool Autosize { get; set; }

        /// <summary>
        /// 原生 autocomplete 属性。
        /// </summary>
        public string Autocomplete { get; set; } = "off";

        /// <summary>
        /// 关联的原生 form 属性。
        /// </summary>
        public string Form { get; set; }

        /// <summary>
        /// 密码输入框是否显示切换图标。
        /// </summary>
        public bool ShowPassword { get; set; }

        /// <summary>
        /// 是否显示字数统计。
        /// </summary>
        public bool ShowWordLimit { get; set; }

        /// <summary>
        /// 字数统计位置，inside 或 outside。
        /// </summary>
        public string WordLimitPosition { get; set; } = "inside";

        /// <summary>
        /// 输入元素样式。
        /// </summary>
        public string InputStyle { get; set; }

        /// <summary>
        /// 是否自动聚焦。
        /// </summary>
        public bool Autofocus { get; set; }

        /// <summary>
        /// Textarea 行数。
        /// </summary>
        public int Rows { get; set; } = 2;

        /// <summary>
        /// 原生 aria-label 属性。
        /// </summary>
        public string AriaLabel { get; set; }

        /// <summary>
        /// 原生 inputmode 属性。
        /// </summary>
        public string Inputmode { get; set; }

        /// <summary>
        /// 原生 tabindex 属性。
        /// </summary>
        public int Tabindex { get; set; } = 0;

        /// <summary>
        /// 值变化时是否触发表单校验。
        /// </summary>
        public bool ValidateEvent { get; set; } = true;
    }
}

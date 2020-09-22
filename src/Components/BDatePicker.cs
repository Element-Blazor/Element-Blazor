using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Element
{
    public class BDatePicker : BInput<DateTime?>
    {
        /// <summary>
        /// 显示的日期格式
        /// </summary>
        [Parameter]
        public string Format { get; set; } = "yyyy-MM-dd";

        /// <summary>
        /// Placeholder
        /// </summary>
        [Parameter]
        public override string Placeholder { get; set; } = "请选择日期";

        /// <summary>
        /// 图标
        /// </summary>
        public override string PrefixIcon { get; set; } = "el-icon-date";

        /// <summary>
        /// 输入框样式类，一般请不要设置该属性
        /// </summary>
        [Parameter]
        public override string Cls { get; set; } = "el-date-editor el-input--prefix el-input--suffix el-date-editor--date";

        /// <summary>
        /// 获取选择的日期
        /// </summary>
        [Parameter]
        public DateTime? Date { get; set; }

        /// <summary>
        /// 日期被选择时触发
        /// </summary>
        [Parameter]
        public EventCallback<DateTime?> DateChanged { get; set; }

        /// <summary>
        /// 时间格式化委托
        /// </summary>
        public override Func<DateTime?, string> Formatter { get; set; }

        protected override void OnInitialized()
        {
            base.OnInitialized();
            Formatter = dt => dt?.ToString(Format);
        }
        [Inject]
        PopupService PopupService { get; set; }
        protected override async Task OnFocusAsync()
        {
            await base.OnFocusAsync();
            if (PopupService.DateTimePickerOptions.Any(x => x.Target.Id == Content.Id))
            {
                return;
            }
            var taskCompletionSource = new TaskCompletionSource<DateTime>();
            PopupService.DateTimePickerOptions.Add(new DateTimePickerOption()
            {
                CurrentMonth = Value.HasValue ? Value.Value : DateTime.Today,
                Target = Content,
                TaskComplietionSource = taskCompletionSource
            });
            Value = (await taskCompletionSource.Task);
            if (ValueChanged.HasDelegate)
            {
                _ = ValueChanged.InvokeAsync(Value);
            }
            if (!Value.HasValue)
            {
                Date = null;
            }
            else
            {
                Date = Value;
            }
            if (DateChanged.HasDelegate)
            {
                _ = DateChanged.InvokeAsync(Date);
            }
            SetFieldValue(Date, true);
        }

        protected override bool ThrowOnInvalidValue { get; } = false;

        protected override void OnChangeEventArgs(ChangeEventArgs input)
        {
            try
            {
                base.OnChangeEventArgs(input);
            }
            catch (FormatException)
            {
                Value = DateTime.Today;
                if (ValueChanged.HasDelegate)
                {
                    _ = ValueChanged.InvokeAsync(Value);
                }
                SetFieldValue(Value, true);
            }
        }
    }
}

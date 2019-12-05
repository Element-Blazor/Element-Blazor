using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace Blazui.Component.Input
{
    public class BDatePicker : BInput<DateTime?>
    {
        [Parameter]
        public string Format { get; set; } = "yyyy-MM-dd";
        [Parameter]
        public override string Placeholder { get; set; } = "请选择日期";

        public override string PrefixIcon { get; set; } = "el-icon-date";

        [Parameter]
        public override string Cls { get; set; } = "el-date-editor el-input--prefix el-input--suffix el-date-editor--date";

        [Parameter]
        public DateTime? Date { get; set; }

        [Parameter]
        public EventCallback<DateTime?> DateChanged { get; set; }

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
                Date = Convert.ToDateTime(Value);
            }
            if (ValueChanged.HasDelegate)
            {
                _ = DateChanged.InvokeAsync(Date);
            }
            SetFieldValue(Date);
        }
    }
}

# ElDateTimePicker Alignment

`ElDateTimePicker` builds on the public `ElDatePickerPanel` and adds time selection for single date-time values and date-time ranges.

## Single Date Time

```razor
<ElDateTimePicker @bind-Value="dateTime"
                  Format="YYYY/MM/DD HH:mm:ss"
                  ValueFormat="YYYY-MM-DD HH:mm:ss"
                  StringValueChanged="value => apiValue = value"
                  DisabledDate="DisablePastDates" />

@code {
    private DateTime? dateTime = DateTime.Now;
    private string apiValue;

    private bool DisablePastDates(DateTime day)
    {
        return day < DateTime.Today;
    }
}
```

## Date Time Range

```razor
<ElDateTimePicker Type="DateTimePickerType.DateTimeRange"
                  RangeValue="@range"
                  RangeValueChanged="value => range = value"
                  ValueFormat="YYYY-MM-DD HH:mm:ss"
                  StringRangeValueChanged="value => apiRange = value"
                  DefaultStartTime="new TimeSpan(9, 0, 0)"
                  DefaultEndTime="new TimeSpan(18, 0, 0)"
                  StartPlaceholder="Start"
                  EndPlaceholder="End" />

@code {
    private IList<DateTime?> range = new List<DateTime?>();
    private IList<string> apiRange = new List<string>();
}
```

## Shortcuts

```razor
<ElDateTimePicker @bind-Value="dateTime" Shortcuts="@shortcuts" />

@code {
    private DateTime? dateTime;
    private DateTimePickerShortcut[] shortcuts =
    {
        new DateTimePickerShortcut { Text = "Now", ValueFactory = () => DateTime.Now },
        new DateTimePickerShortcut { Text = "Tomorrow morning", ValueFactory = () => DateTime.Today.AddDays(1).AddHours(9) }
    };
}
```

Supported picker types are `DateTime` and `DateTimeRange`. `Format` controls display text, while `ValueFormat` controls emitted `StringValue` and `StringRangeValue`; common Element Plus tokens like `YYYY` and `DD` are translated to .NET date-time formats. The popup keeps date and time pending until the user confirms, which lets callers combine calendar selection with explicit `HH:mm:ss` input.

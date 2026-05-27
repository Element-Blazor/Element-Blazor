# ElDatePicker Alignment

`ElDatePicker` now uses the public `ElDatePickerPanel` for popup selection and supports single date/month/year values plus date/month/year ranges.

## Single Date

```razor
<ElDatePicker @bind-Value="date"
              Format="YYYY/MM/DD"
              ValueFormat="YYYY-MM-DD"
              StringValueChanged="value => apiValue = value"
              DisabledDate="DisablePastDates" />

@code {
    private DateTime? date = DateTime.Today;
    private string apiValue;

    private bool DisablePastDates(DateTime day)
    {
        return day < DateTime.Today;
    }
}
```

## Date Range

```razor
<ElDatePicker Type="DatePickerType.DateRange"
              RangeValue="@range"
              RangeValueChanged="value => range = value"
              ValueFormat="YYYY-MM-DD"
              StringRangeValueChanged="value => apiRange = value"
              StartPlaceholder="Start"
              EndPlaceholder="End" />

@code {
    private IList<DateTime?> range = new List<DateTime?>();
    private IList<string> apiRange = new List<string>();
}
```

## Shortcuts

```razor
<ElDatePicker @bind-Value="date" Shortcuts="@shortcuts" />

@code {
    private DateTime? date;
    private DatePickerShortcut[] shortcuts =
    {
        new DatePickerShortcut { Text = "Today", ValueFactory = () => DateTime.Today },
        new DatePickerShortcut { Text = "Yesterday", ValueFactory = () => DateTime.Today.AddDays(-1) }
    };
}
```

Supported picker types are `Date`, `Month`, `Year`, `DateRange`, `MonthRange`, and `YearRange`. `Format` controls display text, while `ValueFormat` controls emitted `StringValue` and `StringRangeValue`; common Element Plus tokens like `YYYY`, `MM`, and `DD` are translated to .NET date formats.

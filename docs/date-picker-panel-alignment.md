# ElDatePickerPanel Alignment

`ElDatePickerPanel` is the public standalone panel for date, month, and year selection.

## Date Panel

```razor
<ElDatePickerPanel @bind-Value="date"
                   DisabledDate="DisablePastDates" />

@code {
    private DateTime? date = DateTime.Today;

    private bool DisablePastDates(DateTime day)
    {
        return day < DateTime.Today;
    }
}
```

## Month And Year Panels

```razor
<ElDatePickerPanel @bind-Value="month" Type="DatePickerPanelType.Month" />
<ElDatePickerPanel @bind-Value="year" Type="DatePickerPanelType.Year" />

@code {
    private DateTime? month = new DateTime(2026, 5, 1);
    private DateTime? year = new DateTime(2026, 1, 1);
}
```

## Footer Slot

```razor
<ElDatePickerPanel @bind-Value="date">
    <FooterContent>
        <ElButton Size="ButtonSize.Small" OnClick="e => date = DateTime.Today">Today</ElButton>
    </FooterContent>
</ElDatePickerPanel>
```

The panel supports `Value/ModelValue`, `Type`, `Disabled`, `Border`, `DisabledDate`, `ValidateEvent`, form validation integration, `OnChange`, `OnPanelChange`, and `OnPick`.

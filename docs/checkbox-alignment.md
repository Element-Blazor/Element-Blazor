# ElCheckbox Alignment

`ElCheckbox`, `ElCheckboxGroup`, and `ElCheckboxButton` support checked, unchecked, indeterminate, disabled, size, bordered checkbox, group value binding, and group selection limits.

## Indeterminate

Use `Indeterminate` or bind `Status` to display the mixed state.

```razor
<ElCheckbox TValue="bool"
            Value="true"
            Indeterminate="true">
    Select all
</ElCheckbox>
```

## Group Limits

`Min` and `Max` restrict how many items may be selected in a group. Limit-disabled items render with the Element Plus `is-disabled` state.

```razor
<ElCheckboxGroup TValue="string"
                 Value="@selected"
                 ValueChanged="items => selected = items.ToList()"
                 Min="1"
                 Max="2"
                 Size="InputSize.Small">
    <ElCheckbox TValue="string" Value="@("a")">A</ElCheckbox>
    <ElCheckbox TValue="string" Value="@("b")">B</ElCheckbox>
    <ElCheckbox TValue="string" Value="@("c")">C</ElCheckbox>
</ElCheckboxGroup>

@code {
    private IList<string> selected = new List<string> { "a" };
}
```

## Button Variant

`ElCheckboxButton` participates in the same group value, limit, disabled, and size behavior.

```razor
<ElCheckboxGroup TValue="string" @bind-Value="selected" Size="InputSize.Large">
    <ElCheckboxButton TValue="string" Value="@("daily")">Daily</ElCheckboxButton>
    <ElCheckboxButton TValue="string" Value="@("weekly")">Weekly</ElCheckboxButton>
</ElCheckboxGroup>
```

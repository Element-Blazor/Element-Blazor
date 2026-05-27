# ElColorPicker Alignment

`ElColorPicker` wraps `ElColorPickerPanel` with the Element Plus-style trigger and popup behavior.

## Basic usage

```razor
<ElColorPicker @bind-Value="color" />

@code {
    private string color = "#409EFF";
}
```

## Alpha and predefined colors

```razor
<ElColorPicker @bind-Value="color"
               ShowAlpha="true"
               ColorFormat="rgb"
               Predefine="@predefineColors"
               Clearable="true" />

@code {
    private string color = "rgba(255, 69, 0, 0.68)";
    private string[] predefineColors =
    {
        "#ff4500",
        "#ff8c00",
        "#ffd700",
        "#90ee90",
        "#00ced1",
        "#1e90ff",
        "#c71585",
        "rgba(255, 69, 0, 0.68)",
        "rgb(255, 120, 0)",
        "hsv(51, 100, 98)",
        "hsva(120, 40, 94, 0.5)",
        "hsl(181, 100%, 37%)",
        "hsla(209, 100%, 56%, 0.73)",
        "#c7158577"
    };
}
```

## Events and methods

```razor
<ElColorPicker @ref="picker"
               @bind-Value="color"
               OnChange="HandleChange"
               OnActiveChange="HandleActiveChange"
               OnClear="HandleClear"
               OnVisibleChange="HandleVisibleChange" />

@code {
    private ElColorPicker picker;
    private string color = "#409EFF";

    private Task HandleChange(string value) => Task.CompletedTask;
    private Task HandleActiveChange(string value) => Task.CompletedTask;
    private Task HandleClear(MouseEventArgs args) => Task.CompletedTask;
    private Task HandleVisibleChange(bool visible) => Task.CompletedTask;

    private Task OpenPanel() => picker.ShowAsync();
    private Task ClosePanel() => picker.HideAsync();
}
```

The picker supports `Size`, `Disabled`, `Clearable`, `ShowAlpha`, `ColorFormat`, `Predefine`, `PopperClass`, `PopperStyle`, `ValidateEvent`, form validation integration, and the panel's supported color formats.

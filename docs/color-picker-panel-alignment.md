# ElColorPickerPanel Alignment

`ElColorPickerPanel` is the standalone panel behind the future `ElColorPicker` popup. It binds a string color value and exposes a small color model through `ElementColor`.

## Basic usage

```razor
<ElColorPickerPanel @bind-Value="color" />

@code {
    private string color = "#409EFF";
}
```

## Alpha and predefined colors

```razor
<ElColorPickerPanel @bind-Value="color"
                    ShowAlpha="true"
                    Predefine="@predefineColors" />

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

## Footer slot and borderless panel

```razor
<ElColorPickerPanel @bind-Value="color" Border="false">
    <FooterContent>
        <ElButton Size="ButtonSize.Small">Apply</ElButton>
    </FooterContent>
</ElColorPickerPanel>
```

Supported value formats include `#rgb`, `#rgba`, `#rrggbb`, `#rrggbbaa`, `rgb()`, `rgba()`, `hsl()`, `hsla()`, `hsv()`, and `hsva()`. Set `ColorFormat` to `hex`, `rgb`, `hsl`, or `hsv` to control emitted values.

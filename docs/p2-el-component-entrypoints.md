# P2 El Component Entrypoints

Element-Blazor exposes Element Plus-style `El*` Razor component entrypoints. Public application, demo, template, and community examples should use only `El*` tags.

## Core Entrypoints

`ElButton`, `ElButtonGroup`, `ElContainer`, `ElHeader`, `ElAside`, `ElMain`, `ElFooter`, `ElIcon`, `ElRow`, `ElCol`, `ElLink`, `ElText`, `ElScrollbar`, `ElSpace`, `ElSplitter`, `ElSplitterPanel`, `ElInput`, `ElInputNumber`, `ElInputTag`, `ElInputOtp`, `ElAutocomplete`, `ElCascader`, `ElRate`, `ElSlider`, `ElTimeSelect`, `ElForm`, `ElFormItem`, `ElFormActionItem`, `ElSelect`, `ElOption`, `ElTable`, `ElTableColumn`, `ElTableColumns`, `ElTableTemplateColumn`, `ElTableDateTimeColumn`, `ElTableCheckBoxColumn`, and `ElTableTreeColumn` are the public core entrypoints.

Additional Element Plus-style entrypoints exist for the rest of the current component surface, including `ElCard`, `ElCheckbox`, `ElRadio`, `ElSwitch`, `ElMenu`, `ElPagination`, `ElTabs`, `ElTabPane`, `ElTag`, `ElTree`, and `ElUpload`.

## Table Column Shape

`ElTableColumn` can be used directly under `ElTable`:

```razor
<ElTable DataSource="Datas" AutoGenerateColumns="false">
    <ElTableColumn Property="@nameof(Row.Name)" Text="Name" />
    <ElTableTemplateColumn Text="Action">
        <ElButton Type="ButtonType.Primary" Size="ButtonSize.Small">Edit</ElButton>
    </ElTableTemplateColumn>
</ElTable>
```

The grouped column shape remains available:

```razor
<ElTable DataSource="Datas" AutoGenerateColumns="false">
    <ElTableColumns>
        <ElTableColumn Property="@nameof(Row.Name)" Text="Name" />
    </ElTableColumns>
</ElTable>
```

## Public Example Policy

Public examples in `demo`, `template`, and `community` use `El*` tags. Element Plus-style parameter names are used in examples:

| Public API | Use |
| --- | --- |
| `Disabled` | Disable controls. |
| `Loading` | Button loading state. |
| `Plain` | Plain button style. |
| `Round` | Round button style. |
| `Circle` | Circle button style. |
| `Clearable` | Input clear button. |
| `Required` | Form item required state. |
| `Error` | Form item immediate error message. |
| `LabelPosition` | Form label alignment. |
| `Prop` | Form item model property. |
| `Model` | Form model object. |
| `Rules` | Form-level validation rules keyed by `Prop`/`Name`. |
| `Validate` / `ValidateField` | Validate all fields or selected fields. |
| `ResetFields` / `ClearValidate` | Reset field values or clear validation state. |
| `ScrollToFieldAsync` | Scroll a form field into view. |
| `OnSubmitForm` / `OnValidSubmitForm` / `OnInvalidSubmitForm` | Typed form submit callbacks with the `ElForm` instance and validation result. |
| `LabelContent` / `ErrorContent` | Form item label and error render fragments. |

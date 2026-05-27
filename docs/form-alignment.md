# ElForm Alignment

`ElForm` now has a tested closed loop for Element Plus-style form workflows:

| Area | Status |
| --- | --- |
| Model binding | `Model`/`Value` support direct and nested `Prop` paths such as `User.Name`. Field changes update the model and `Values`. |
| Validation | Field `Rules`, form `Rules`, legacy `Validations`, `Required`, and model `DataAnnotations` are merged. Async validation is available through `IAsyncValidationRule`. |
| Submit | `OnSubmit`, `OnValidSubmit`, and `OnInvalidSubmit` remain supported. Typed submit callbacks are available through `OnSubmitForm`, `OnValidSubmitForm`, and `OnInvalidSubmitForm`. |
| Reset | `ResetFields` restores field UI, `Values`, and the bound model. |
| Clear | `ClearValidate` clears field validation state without changing values. |
| Scroll | `ScrollToFieldAsync` and `ScrollToError` scroll to field wrappers or registered inputs. |
| Generated form | `EntityType` honors `DisplayAttribute`/`DisplayNameAttribute` labels and `RequiredAttribute` metadata. |
| Blazor integration | A cascaded `EditContext` model is used when `Model`/`Value` is not set. |
| Slots | `ElFormItem` supports `LabelContent` and `ErrorContent` render fragments. |

The closure is covered by `test/Element.ComponentTests/ElFormTests.cs`.

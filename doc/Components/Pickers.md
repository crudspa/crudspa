# Components | Pickers

Pickers handle structured values that aren't free-form text: dates, date-times, date ranges, colors, and file names. Consistent parsing and representation here prevents many downstream bugs.

Crudspa picker components keep these values strongly typed and predictable.

Some of these are thin Radzen wrappers, and some are Crudspa compositions around typed values and Blazor primitives.

## Component Catalog

 Component | Purpose | Value Type | Implementation
 --- | --- | --- | ---
 `DatePicker` | date-only selection | `DateOnly?` | Radzen `RadzenDatePicker` wrapper
 `DateTimePicker` | date-time selection | `DateTimeOffset?` | Radzen `RadzenDatePicker` wrapper
 `DateFilter` | relative or custom date-range filter UI | `DateRange` | Crudspa filter UI with Radzen date pickers for custom ranges
 `ColorPicker` | color selection with hex storage and rgba editing | `String?` (hex) | Radzen `RadzenColorPicker` wrapper
 `FilePicker` | filename selection helper | `String?` filename | Blazor `InputFile` helper

## Default Approach

Use strongly typed controls and bind directly to model properties:

```razor
<Field Label="Next Run"
       Size="Field.Sizes.Small">
    <DateTimePicker ReadOnly="Model.ReadOnly"
                    @bind-Value="Model.Entity.NextRun" />
</Field>

<Field Label="Added"
       Size="Field.Sizes.Medium">
    <DateFilter Range="Model.Search.AddedRange" />
</Field>
```

## Option Reference

### `DatePicker`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Value` / `ValueChanged` | selected date | supports `@bind-Value`
 `Format` | display format string | required parameter in component
 `ReadOnly` | disables picker | default `false`

### `DateTimePicker`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Value` / `ValueChanged` | selected date-time | supports `@bind-Value`
 `Format` | display format string | default `"g"`
 `ReadOnly` | disables picker | default `false`

### `DateFilter`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Range` | mutable date range object | required (`DateRange`)

`DateRange.Types` supports:

* `Any`
* `Custom`
* `InTheLastDay`
* `InTheLastWeek`
* `InTheLastMonth`
* `InTheLastThreeMonths`
* `InTheLastSixMonths`
* `InTheLastYear`

### `ColorPicker`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Value` / `ValueChanged` | hex color value | component converts rgba to hex and back
 `ReadOnly` | disables interaction | default `false`

### `FilePicker`

 Parameter | Purpose | Notes
 --- | --- | ---
 `FileName` / `FileNameChanged` | selected filename | supports `@bind-FileName`
 `ReadOnly` | disables browse button | default `false`

`FilePicker` also exposes `FileInputReference` for advanced cases.

## Radzen-Backed Wrappers

`DatePicker`, `DateTimePicker`, and `ColorPicker` are intentionally thin Radzen wrappers. `DateFilter` is a Crudspa filter component that uses Radzen date pickers only for the custom-range portion. This keeps the public API small and typed while leaving the full bundled Radzen library available for more specialized cases.

## Integration Patterns

* Pair picker controls with `Field` for consistent label and layout behavior.
* Use `DateFilter` with list/find model search objects.
* Keep date/time values typed in model contracts to avoid string parsing issues.
* Use media uploaders for real uploads; keep `FilePicker` for lightweight filename capture only.

## Practical Guidance

* Use `DatePicker` for business dates without time-of-day semantics.
* Use `DateTimePicker` for scheduling or timestamp fields.
* For search filters, prefer `DateFilter` over separate start/end date fields.
* Keep color values stored as hex strings if they need to round-trip through APIs or persistence.

## Common Questions

### Should I use `DatePicker` or `DateTimePicker` for local time values?

If you need time-of-day, use `DateTimePicker`. If you only need the date, use `DatePicker`.

### Can `DateFilter` be used outside list/find pages?

Yes, but it's primarily optimized for search criteria and report filters.

### Why not use `FilePicker` for uploads?

`FilePicker` only captures selected file name. It doesn't upload content or track progress.

## Tradeoffs

Picker wrappers intentionally keep API surfaces small, and several are deliberately thin Radzen wrappers. You lose some low-level control compared to direct third-party picker usage, but gain stronger consistency and safer model binding.

## Next Steps

* [Components | Dropdowns](Dropdowns.md)
* [Components | Media](Media.md)
* [Types | Date/Time](../Types/Datetime.md)
* [Documentation Index](../ReadMe.md)

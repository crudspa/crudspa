# Components | Selections

Selection controls look simple but drive many business rules. Null behavior, orientation, and data-shape assumptions must stay consistent to avoid subtle bugs.

Crudspa selection components cover boolean, enum, lookup, and multi-select use cases with predictable contracts.

Most of this family uses native markup because the behavior is already simple and Crudspa-specific. `MultiSelect` is the main Radzen-backed exception.

## Component Catalog

 Component | Purpose | Data Shape | Implementation
 --- | --- | --- | ---
 `Checkbox` | single boolean toggle | `Boolean?` | native html checkbox
 `CheckedList` | visible list of selectable items | `ObservableCollection<Selectable>` | Crudspa composition over `Checkbox`
 `Radio` | single lookup selection with optional null option | `IEnumerable<INamed>` | native html radio group
 `BoolRadio` | explicit true/false radio pair | `Boolean?` | native html radio group
 `EnumRadio<TEnum>` | enum radio group | enum type | native html radio group
 `MultiSelect` | compact multiple-selection dropdown chips | `IEnumerable<Named>` + `List<Guid?>` | Radzen `RadzenDropDown` wrapper

## Default Approach

Prefer typed selection components over custom input markup:

```razor
<Field Label="Recurrence Pattern"
       Size="Field.Sizes.Unspecified">
    <EnumRadio TEnum="JobSchedule.RecurrencePatterns"
               ReadOnly="Model.ReadOnly"
               @bind-SelectedValue="Model.Entity.RecurrencePattern" />
</Field>

<Field Label="Job Types"
       Size="Field.Sizes.Medium">
    <MultiSelect LookupValues="Model.JobTypeNames"
                 @bind-SelectedValues="Model.Search.Types" />
</Field>
```

## Option Reference

### `Checkbox`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Value` / `ValueChanged` | selected boolean value | supports `@bind-Value`
 `AlsoValueChanged` | additional callback after standard value change | useful for side effects
 `ReadOnly` | disables checkbox | default `false`
 `Label` | checkbox text | optional
 `ModifierClass` | additional css class | optional

### `CheckedList`

 Parameter | Purpose | Options
 --- | --- | ---
 `Selectables` | item set with `Selected` flag | required
 `ReadOnly` | disables item toggles | default `false`
 `Height` | list height class | `Short`, `Average`, `Tall`, `Full`
 `WidthClass` | custom width class | optional

### `Radio`

 Parameter | Purpose | Notes
 --- | --- | ---
 `LookupValues` | available options | required
 `Value` / `ValueChanged` | selected id | supports `@bind-Value`
 `AllowNull` / `NullText` | explicit no-selection option | optional
 `Vertical` | vertical layout mode | default horizontal
 `RadioCssClass` | custom class | optional
 `ReadOnly` | disables options | default `false`

### `BoolRadio` and `EnumRadio<TEnum>`

 Parameter | Purpose | Options
 --- | --- | ---
 `SelectedValue` / `SelectedValueChanged` | selected value | supports two-way binding
 `ReadOnly` | disables selection | default `false`
 `Orientation` | layout mode | `Horizontal`, `Vertical`
 `RadioCssClass` | custom class | optional

`BoolRadio` also supports `TrueLabel` and `FalseLabel`.

### `MultiSelect`

 Parameter | Purpose | Notes
 --- | --- | ---
 `LookupValues` | available values | required
 `SelectedValues` / `SelectedValuesChanged` | selected ids list | supports `@bind-SelectedValues`

`MultiSelect` is a focused wrapper over Radzen's chips-style multi-select dropdown. If you need more advanced multi-select behavior than Crudspa exposes here, the full bundled Radzen library is available directly.

## Integration Patterns

* Use `CheckedList` for highly visible permission/role selections.
* Use `MultiSelect` when space is limited and large lists are acceptable in dropdown form.
* Use `EnumRadio<TEnum>` for small enum sets where seeing all options improves decision speed.
* Keep selection collections (`Selectable`, `Named`) in model state so read views can reuse the same data.

## Practical Guidance

* Use `BoolRadio` when labels matter more than simple on/off toggles.
* Use `Checkbox` for quick flags and secondary options.
* Keep null behavior explicit for optional relationships (`AllowNull` with clear `NullText`).
* Keep orientation consistent within a form section for readability.

## Common Questions

### `Checkbox` or `BoolRadio` for yes/no fields?

Use `BoolRadio` when explicit labels improve clarity. Use `Checkbox` for lightweight flags.

### Should enum choices be dropdown or radio?

If option count is small and clarity matters, use radio. If space is tight or options are many, use dropdown.

### How do I handle many relationship options?

Use `CheckedList` for always-visible choices or `MultiSelect` for compact UI.

## Tradeoffs

Selection wrappers assume framework data shapes (`Selectable`, `INamed`, enum types). This constraint is intentional and improves consistency across modules while keeping the vendor-backed surface area small.

## Next Steps

* [Components | Dropdowns](Dropdowns.md)
* [Components | Forms](Forms.md)
* [Types | Boolean](../Types/Boolean.md)
* [Documentation Index](../ReadMe.md)

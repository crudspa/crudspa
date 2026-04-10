# Components | Dropdowns

Lookup selection is everywhere in CRUD forms. The challenge isn't just showing choices, but keeping null handling, filtering, and display conventions consistent across modules.

Crudspa provides multiple dropdown variants with clear intent boundaries.

This family mixes native selects with Radzen-backed wrappers, depending on how much behavior the scenario needs.

## Component Catalog

 Component | Best For | Data Shape | Implementation
 --- | --- | --- | ---
 `Dropdown` | simple lookup lists with native select behavior | `IEnumerable<INamed>` | native html `select`
 `ComboBox` | large lists that need filtering | `IEnumerable<INamed>` | Radzen `RadzenDropDown` wrapper
 `IconDropdown` | lookups with icon preview | `IEnumerable<IconFull>` | Radzen `RadzenDropDown` wrapper
 `EnumDropdown<TEnum>` | enum values without custom lookup list creation | enum type | native html `select`
 `TimeZonePicker` | system time-zone selection | `String` time zone id | native html `select`

## Default Approach

Start with `Dropdown`. Move to `ComboBox` only when users need in-control filtering.

```razor
<Field Label="Device"
       Size="Field.Sizes.Medium">
    <Dropdown LookupValues="Model.DeviceNames"
              ReadOnly="Model.ReadOnly"
              AllowNull="true"
              NullText="[Any]"
              @bind-Value="Model.Entity.DeviceId" />
</Field>

<Field Label="Book"
       Size="Field.Sizes.Larger">
    <ComboBox LookupValues="Model.SummerBookNames"
              ReadOnly="Model.ReadOnly"
              @bind-Value="context.BookId" />
</Field>
```

## Option Reference

### `Dropdown`, `ComboBox`, `IconDropdown`

 Parameter | Purpose | Notes
 --- | --- | ---
 `LookupValues` | available choices | required
 `Value` / `ValueChanged` | selected id | supports `@bind-Value`
 `ReadOnly` | disables interaction | standard form read mode integration
 `AllowNull` | allows empty selection | useful for optional relation filters
 `NullText` | label for null selection | defaults to `[None]`

`ComboBox` defaults to case-insensitive filtering. `IconDropdown` uses custom value/list templates so icon and name render together.

### `EnumDropdown<TEnum>`

 Parameter | Purpose | Notes
 --- | --- | ---
 `SelectedValue` / `SelectedValueChanged` | selected enum value | supports `@bind-SelectedValue`
 `ReadOnly` | disables select | standard integration with form read mode

Enum labels are rendered with inserted spaces (`InTheLastMonth` -> `In The Last Month`).

### `TimeZonePicker`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Value` / `ValueChanged` | selected `TimeZoneInfo.Id` | supports `@bind-Value`
 `ReadOnly` | disables select | read mode support
 `AllowNull` / `NullText` | optional no-selection state | useful on optional time zone fields

Time zones come from `TimeZoneInfo.GetSystemTimeZones()`.

## Radzen-Backed Wrappers

`ComboBox` and `IconDropdown` are thin wrappers over Radzen dropdown controls. Crudspa narrows the API to the lookup conventions used across modules, while still leaving the full bundled Radzen library available when a page needs more specialized dropdown behavior.

## Integration Patterns

* Use lookup components inside `Field` so labels and spacing remain consistent.
* Pair with `Labeled` in read views for consistent detail card display.
* Use `AllowNull` in find/filter UIs to represent "any value" cleanly.
* Keep relation IDs strongly typed (`Guid?`) in model entities and search contracts.

## Practical Guidance

* If list size is small and static, prefer native `Dropdown` for lower complexity.
* Use `ComboBox` for long, user-searchable lists.
* Use `IconDropdown` only when icon context materially improves selection speed.
* Prefer `EnumDropdown<TEnum>` over manually duplicated enum lookup arrays.
* Prefer `TimeZonePicker` over hand-maintained time zone lists.

## Common Questions

### Which should be my default: `Dropdown` or `ComboBox`?

`Dropdown` should be default. Use `ComboBox` only when list size and search needs justify extra behavior.

### Can I use `EnumDropdown<TEnum>` for nullable enum values?

Use nullable enum properties with a wrapper pattern or alternate control when null choice must be explicit.

### Should I load lookup values inside the component?

No. Keep fetching and caching in models/services; pass values into components.

## Tradeoffs

Crudspa lookup components favor predictable contracts over full per-control configurability. That tradeoff reduces local UI experiments, but keeps relation-edit behavior uniform across the platform and keeps vendor swaps practical.

## Next Steps

* [Components | Selections](Selections.md)
* [Components | Pickers](Pickers.md)
* [Components | Forms](Forms.md)
* [Documentation Index](../ReadMe.md)

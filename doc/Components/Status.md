# Components | Status

Status is more than a plain label in CRUD screens. It often carries workflow meaning, validation hints, publishing state, or operational health, so users should be able to spot it quickly in both read and edit views.

Crudspa's status components standardize that pattern with one display component, one edit component, and an optional shared color-mapping helper.

## Component Catalog

 Component | Purpose | Data Shape | Notes
 --- | --- | --- | ---
 `StatusDisplay` | read-only status pill | `Guid?` id + `String` name + optional css class | renders nothing when `Name` is empty
 `StatusEdit` | visible button-group status picker | `IEnumerable<INamed>` + `Guid?` selected id | best for small lookup-backed status sets
 `ContentStatusColors` | shared color resolver for common content states | `Func<Guid?, String?, String?>` | maps draft/complete/retired ids or names to built-in css classes

## Default Approach

Use `StatusEdit` in edit surfaces and `StatusDisplay` in read surfaces:

```razor
<Field Label="Status"
       Size="Field.Sizes.Unspecified">
    <StatusEdit LookupValues="Model.ContentStatusNames"
                ReadOnly="Model.ReadOnly"
                ColorClassFrom="ContentStatusColors.For"
                @bind-Value="Model.Entity.StatusId" />
</Field>

<Labeled CssClass="status"
         Width="6em"
         Label="Status">
    <StatusDisplay Id="@context.Page.StatusId"
                   Name="@context.Page.StatusName"
                   ColorClassFrom="ContentStatusColors.For" />
</Labeled>
```

This is the default path for lookup-backed content states because the same resolver can color both edit buttons and read-only pills.

## Option Reference

### `StatusDisplay`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Name` | text shown inside the status pill | required in practice; component doesn't render without it
 `Id` | optional status id | used by resolver functions
 `ColorClass` | explicit css class for the pill | takes precedence over `ColorClassFrom`
 `ColorClassFrom` | shared resolver function | receives `(id, name)` and returns a css class or `null`
 `CssClass` | additional wrapper classes | appended to resolved color class

### `StatusEdit`

 Parameter | Purpose | Notes
 --- | --- | ---
 `LookupValues` | available status options | required; each item must implement `INamed`
 `Value` / `ValueChanged` | selected status id | supports `@bind-Value`
 `ReadOnly` | disables selection changes | keeps selected option visually distinct
 `AllowNull` | shows an explicit no-selection option | default `false`
 `NullText` | label for the null option | useful with `AllowNull`
 `ColorClassFrom` | fallback resolver function | used when lookup items don't already provide a css class
 `CssClass` | additional classes for the button group | optional

## Color Resolution

`StatusDisplay` and `StatusEdit` resolve color a little differently:

* `StatusDisplay` uses `ColorClass` first, then falls back to `ColorClassFrom`.
* `StatusEdit` uses `ICssClass.CssClass` from each lookup item first, then falls back to `ColorClassFrom`.

That gives you two clean integration paths:

* Use `ColorClassFrom="ContentStatusColors.For"` when color is derived from shared status meaning.
* Use lookup/result types that already carry `CssClass` when the status style comes from data, such as job statuses.

For example:

```razor
<StatusDisplay Name="@(card.Entity.StatusName)"
               ColorClass="@card.Entity.StatusCssClass" />
```

## Built-In Status Classes

Crudspa's default styles already recognize these status-oriented classes:

* `content-draft`
* `content-complete`
* `content-retired`
* `status-pending`
* `status-running`
* `status-completed`
* `status-failed`
* `status-canceled`

If you use a different class name, the components still work, but the final appearance depends on your own stylesheet rules.

## Integration Patterns

* Use `StatusEdit` inside `Field` so status selection lines up with the rest of the form vocabulary.
* Use `StatusDisplay` inside `Labeled CssClass="status"` to match the tighter read-card styling used across the platform.
* Prefer lookup types that include ids and names even when only the name is shown today. That keeps color resolution and future workflow hooks easy.
* For content authoring, fetch content statuses once into model state and reuse the same lookup list for defaults, edit controls, filters, and read views.

## Practical Guidance

* Use `StatusEdit` when status should stay visible at all times. Because it renders every option as a button, it's better for small sets than large taxonomies.
* Keep the status vocabulary stable. A status control works best when each option has clear workflow meaning, not just alternate wording.
* Prefer shared color mappings over one-off local class decisions so status meaning stays consistent across lists, cards, and forms.
* Set `AllowNull` only when a missing status is a real business state, not just a temporary loading gap.

## Common Questions

### `Radio` or `StatusEdit`?

Use `StatusEdit` when status is an important workflow signal and color should reinforce meaning. Use `Radio` when you only need ordinary single selection without status styling.

### Do I need both `Id` and `Name` for `StatusDisplay`?

`Name` is the key requirement because it's the visible text. `Id` is optional but useful when color resolution depends on stable identifiers instead of display names.

### Should I use `ColorClass` or `ColorClassFrom`?

Use `ColorClass` when the data already includes the final class to render. Use `ColorClassFrom` when you want a shared mapping function that multiple features can reuse.

## Tradeoffs

These components intentionally favor visibility and consistency over compactness. `StatusEdit` is stronger than a dropdown for small status sets, but it's not the right fit for large lookups. The coloring story is also convention-based, so teams get the most value when they share a small, stable css class vocabulary.

## Next Steps

* [Components | Forms](Forms.md)
* [Components | Selections](Selections.md)
* [Styling | Theming](../Styling/Theming.md)
* [Documentation Index](../ReadMe.md)

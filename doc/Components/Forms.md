# Components | Forms

Forms are where CRUD complexity accumulates: read/edit toggles, confirmation flows, labels, field sizing, nested collections, and async save behavior.

Crudspa form components provide a single composition model so every module handles these concerns the same way.

## Component Catalog

 Component | Purpose | Primary Integration
 --- | --- | ---
 `Field` | labeled wrapper for any control or display value | shared sizing/padding/orientation vocabulary
 `Form<T>` | read/edit form shell with built-in action handling | `FormModel<T>`, optional modal edit mode
 `Card<T>` | reusable read card with optional delete confirmation | `ICardModel<T>`, `ModalModel` confirmation
 `Labeled` | typed read-only display value | session-aware date/time localization
 `LabeledHtml` | read-only html display | `UserHtml` rendering
 `LabeledSelections` | read-only selected options list | `Selectable` collection
 `LabeledRelations` | read-only relation list with optional "all" state | `Selectable` collection
 `LabeledBatch<T>` | read-only repeated item display | typed collection + render fragment

## Default Approach

Use `Form<T>` as the default edit surface for entity cards:

```razor
<Form Model="form"
      ModalEdit="true"
      ModalEditTitle="Section Editor"
      SaveRequested="() => Model.Save(form.Entity.Id)"
      CancelRequested="() => Model.Cancel(form.Entity.Id)"
      DeleteRequested="() => Model.Delete(form.Entity.Id)">
    <ReadView>
        <SectionDisplay Section="form.Entity.Section" />
    </ReadView>
    <EditView>
        <SectionEdit Model="form.Entity" />
    </EditView>
</Form>
```

This pattern keeps model state and visual behavior aligned across list, many-edit, and standalone edit screens.

## `Field` Option Reference

`Field` is the core layout primitive for both edit and read content.

 Parameter | Purpose | Options
 --- | --- | ---
 `Label` | field label text | string
 `LabelActions` | inline actions next to label | render fragment
 `Container` | wrapping element | `Label`, `Div`
 `Size` | width class | `Pico`, `Nano`, `Tiny`, `Medium`, `Wide`, `Full`, `Star`, and others
 `Orientation` | label/content orientation | `Vertical`, `Horizontal`
 `Padding` | spacing variant | `Default`, `None`, `Tight*`
 `ChildContent` | field content | required

Use shared sizes aggressively. It is one of the biggest consistency levers in Crudspa screens.

## `Form<T>` Option Reference

 Parameter | Purpose | Notes
 --- | --- | ---
 `Model` | form state for entity, read mode, alerts, confirmation | required
 `ReadView` / `EditView` | display and editor fragments | required
 `ModalEdit` | opens edit in local modal | useful in dense list/many surfaces
 `ModalEditTitle` | modal title override | optional
 `SupportsDelete` | enables delete action path | default `true`
 `SaveRequested` / `CancelRequested` / `DeleteRequested` | action callbacks | required for mutation flows
 `MoveUpRequested` / `MoveDownRequested` | reorder actions in orderable forms | optional
 `ReadViewContainer` | read card wrapper mode | `TitleAndWrappedValues`, `None`
 `Tight` | compact card rendering | useful in nested collections

## `Card<T>` And Confirmation Behavior

`Card<T>` includes an optional delete confirmation modal backed by `Model.ConfirmationModel`. When `SupportsDelete=true`, the card can show a standardized confirmation flow without extra page wiring.

This is why `List<T>` and `Form<T>` can stay small while still handling destructive actions safely.

## Read View Helpers

Use `Labeled*` components to keep read mode consistent:

* `Labeled` for scalar values, date/time, number, links, and booleans.
* `LabeledHtml` for trusted html output.
* `LabeledSelections` and `LabeledRelations` for option collections.
* `LabeledBatch<T>` for repeated item summaries.

`Labeled` converts `DateTimeOffset` values to session time zone through `ISessionState`.

## Framework Integration

Form components align with model contracts used throughout Crudspa:

* `FormModel<T>` tracks `ReadOnly`, `IsNew`, `Hidden`, `SortIndex`, and `ConfirmationModel`.
* `ScreenModel` features (`Alerts`, `WithWaiting`, `WithAlerts`) flow directly into `Form<T>` via `Waiter`.
* `List<T>`, `Many<T>`, and orderable variants compose forms/cards instead of duplicating behavior.

## Practical Guidance

* Use `ModalEdit` for dense many-edit screens where inline edit would be noisy.
* Keep `ReadView` concise and use `Labeled*` helpers for consistency.
* Use `LabelActions` for field-specific actions instead of adding detached buttons nearby.
* Avoid raw HTML layout for common fields unless there is a clear exception.

## Common Questions

### When should I use `Card<T>` directly?

When you need custom card composition outside of `List<T>` or `Many<T>`, but still want shared confirmation and button patterns.

### Should read views use raw tags instead of `Labeled*`?

Use `Labeled*` by default. Raw tags are best reserved for highly custom visual content.

### How do I keep modal edit and page scroll context stable?

Use built-in `Form<T>` modal edit behavior. It handles modal close and return-to-entity scroll behavior.

## Tradeoffs

Form primitives are intentionally structured. You trade some local markup freedom for a consistent, model-driven UI contract that scales much better across large CRUD solutions.

## Next Steps

* [Components | Layouts](Layouts.md)
* [Components | Lists](Lists.md)
* [Patterns | Edit](../Patterns/Edit.md)
* [Documentation Index](../ReadMe.md)

# Components | Feedback

Users should always know what the system is doing: loading, failing, validating, or returning no data. Inconsistent feedback is one of the top causes of poor CRUD UX.

Crudspa standardizes these states around `ScreenModel` and a small feedback component set.

## Component Catalog

 Component | Purpose | Primary Integration
 --- | --- | ---
 `Waiter` | loading overlay plus alert host around content | `ScreenModel`
 `Alerts` | renders message/error list bubbles with dismiss support | `ObservableCollection<Alert>`
 `NoRecords` | standardized empty-list text | entity name string
 `SplashScreen` | startup loading screen for app shell boot | root app layout

## Default Approach

Use `Waiter` around interactive content and drive it from model helpers:

```razor
<Waiter Model="Model">
    <Filters>
        <Fields>
            ...
        </Fields>
        <Buttons>
            ...
        </Buttons>
    </Filters>
</Waiter>

@if (Model.Cards.IsEmpty())
{
    <NoRecords EntityName="jobs" />
}
```

In model code, use `WithWaiting` and `WithAlerts` so feedback is automatic and consistent.

## `ScreenModel` Integration

`ScreenModel` is the key feedback contract used across find/list/edit models.

 Helper | Purpose
 --- | ---
 `WithWaiting(...)` | sets `Waiting=true`, updates `WaitingOn`, wraps async call
 `WithAlerts(...)` | clears dismissible alerts (by default), adds errors on failed responses
 `WithMany(...)` | runs multiple tasks under one waiting state
 `IsValid(...)` | pushes validation errors to alerts

Because `Waiter` renders both loading UI and `<Alerts Items="Model.Alerts" />`, these helpers eliminate repetitive page-level feedback wiring.

## Option Reference

### `Waiter`

 Parameter | Purpose
 --- | ---
 `Model` | `ScreenModel` instance containing waiting and alerts state
 `ChildContent` | content rendered under overlay and alert stack

### `Alerts`

 Parameter | Purpose
 --- | ---
 `Items` | alert collection to render and observe for collection changes

### `NoRecords`

 Parameter | Purpose
 --- | ---
 `EntityName` | natural language entity label shown in empty-state text

## Alert Types

Framework `Alert` supports these standard types:

* `Error`
* `Warning`
* `Success`
* `Tip`
* `Lock`

Use these types instead of custom one-off color classes. The framework maps each type to consistent iconography and styling.

## Practical Guidance

* Keep `Waiter` at the pane/screen boundary, not around tiny fragments.
* Use `SplashScreen` only for app startup, not for in-page waiting.
* Set meaningful `WaitingOn` text for long operations (`Saving...`, `Fetching...`, `Moving...`).
* Keep `NoRecords` entity names human-readable and plural.

## Common Questions

### Should every page show `Waiter`?

Most interactive pages should. It gives consistent loading and error display with minimal markup.

### Can I add non-error alerts?

Yes. Add `Alert` items directly to `Model.Alerts` for success or guidance messages.

### How do I avoid alert duplication?

Use `WithAlerts` defaults so dismissible alerts are cleared before new requests.

## Tradeoffs

The feedback model assumes your page models derive from `ScreenModel`. That constraint is deliberate and keeps status behavior predictable across all modules.

## Next Steps

* [Components | Dialogs](Dialogs.md)
* [Components | Lists](Lists.md)
* [Documentation Index](../ReadMe.md)

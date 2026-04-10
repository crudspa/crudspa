# Components | Tabs

Tabbed navigation is common in pane-oriented CRUD interfaces. Problems usually appear when tab state isn't URL-aware, nested tabs collide, or lazy loading is handled inconsistently.

Crudspa tabs are model-backed and navigation-aware by default.

## Component Catalog

 Component | Purpose | Primary Integration
 --- | --- | ---
 `Tabs` | tab container and active tab coordination | `TabsModel`, query string state, event bus updates
 `Tab` | declarative tab registration | key/title/lazy/content metadata
 `TabsModel` | active tab state, query parameter updates, dynamic tab handling | `INavigator`
 `TabScope` | nested-tab query-key scoping | automatic `tab`, `tab2`, `tab3`, ...

## Default Approach

The main public example in this repo is the dynamic path used by `TabbedPanesDisplay`. It builds a `TabsModel` from pane metadata and lets the `Tabs` component handle query-string sync and active-tab state:

```razor
<Tabs Model="Tabs"
      Path="@Path"
      Vertical="true"
      HideTabs="@(IsNew)" />
```

That same component also supports declarative child `Tab` components for fixed tab sets, but the metadata-driven model is the more typical Crudspa path in pane-heavy shells.

## Option Reference

### `Tabs`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Path` | current route path for query-state sync | required
 `Model` | optional external tab model | use for dynamic tabs
 `ChildContent` | declarative tabs | use for static tabs
 `QueryKey` | query parameter key for active tab | defaults by scope (`tab`, `tab2`, ...)
 `LockOnNew` | keeps initial tab when new-state flow shouldn't switch tabs | useful in create workflows
 `HideTabs` | hides tab header list | content still rendered
 `Vertical` | vertical tabs layout | optional

`Tabs` can't use both `Model` and `ChildContent` at once.

### `Tab`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Key` | stable tab identifier | required
 `Title` | tab label | required
 `Lazy` | defer content render until first activation | default `false`
 `ChildContent` | tab content fragment | optional when using pane plugin metadata

### `TabsModel`

 Behavior | Description
 --- | ---
 `Initialize` | sets available tabs and resolves initial active key
 `SetActive` | activates tab and updates query parameter
 `ApplyPath` | reads query parameter and updates active key when route changes

## Query String And Nested Tab Behavior

`Tabs` listens for `QueryStringChanged` and keeps active tab synchronized with navigation state.

Nested tabs use `TabScope` automatically:

* first tab level uses query key `tab`
* second level uses `tab2`
* third level uses `tab3`

This prevents nested tab collisions without manual key management.

## Framework Integration

Tabs integrate with broader framework concerns:

* `INavigator` updates query parameters on tab changes.
* `ErrorRecover` wraps tab content, so one tab failure doesn't crash the entire shell.
* pane plugins can render through tab metadata (`PaneTypeDisplayView`) when content isn't direct fragment markup.

## Practical Guidance

* Use static tabs for compile-time-known screens.
* Use `TabsModel` for server-driven or plugin-driven tab collections.
* Use `Lazy=true` for expensive tabs that users may never open.
* Keep tab keys stable and route-safe.

## Common Questions

### Should I always use `TabsModel`?

No. Declarative `Tab` children are simpler for static sets.

### How do I keep the first tab fixed during new-item flows?

Use `LockOnNew` so query-string updates don't switch tabs unexpectedly.

### Can tabs host plugin components?

Yes. The framework supports pane display plugin rendering within tab panes.

## Tradeoffs

Crudspa tabs are heavier than purely local state tabs because they are URL-aware and model-driven. That added structure is intentional and supports deep links, browser navigation, and consistent pane behavior.

## Next Steps

* [Components | Trees](Trees.md)
* [Components | Domain](Domain.md)
* [Documentation Index](../ReadMe.md)

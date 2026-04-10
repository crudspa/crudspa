# Components | Dialogs

CRUD+SPA systems rely on overlays for edit forms, confirmations, viewers, and error recovery. Without a shared dialog model, focus behavior, waiting states, and close handling drift across screens.

Crudspa uses a single modal stack with model-driven state and a shared app-level host for common viewer modals.

## Component Catalog

 Component | Purpose | Primary Integration
 --- | --- | ---
 `Modal` | Standard overlay shell with title, content, and optional buttons | `ModalModel` (or subclasses)
 `ErrorRecover` | Error boundary that logs once and shows a fallback error panel | wraps app shell or pane content
 `RootModalsCore` | Shared host for image/pdf viewers | `ImageViewerModel`, `PdfViewerModel`
 `ModalModel` | Base model for visibility, title, alerts, waiting, and scroll-to-modal behavior | `IScrollService`, `ScreenModel`

## Default Approach

Use model-backed modals for all workflow dialogs:

```razor
<Modal Model="EditProfileModel"
       TopMargin="6em"
       Width="32em"
       Title="Edit Profile"
       Scope="Modal.ModalScope.Global">
    <Content>
        <EditProfile Model="EditProfileModel" />
    </Content>
    <Buttons>
        <ButtonOk Clicked="() => EditProfileModel.Save()" />
        <ButtonCancel Clicked="() => EditProfileModel.Hide()" />
    </Buttons>
</Modal>
```

The model provides alert and waiting behavior automatically through the modal's embedded `Waiter`.

## `Modal` Option Reference

 Parameter | Purpose | Default
 --- | --- | ---
 `Model` | modal state (`Visible`, `Title`, alerts, waiting) | required
 `Content` | main modal body fragment | required
 `Buttons` | optional button well | none
 `Title` | initial title value | empty
 `Scope` | `Local` or `Global` stacking behavior | `Local`
 `Sizing` | `Auto`, `Fluid`, or `Full` layout mode | `Auto`
 `Width` | explicit width for non-full sizing | `auto`
 `TopMargin` | vertical offset from top | `.25em`
 `ShowOverlay` | toggles overlay dimming | `true`
 `NoPadding` | removes standard content padding | `false`
 `ZIndex` | overlay stacking level | `300`

## Scope And Sizing Guidance

* `Scope="Local"`: modal belongs to a local pane or section.
* `Scope="Global"`: modal should sit above full shell content.
* `Sizing="Fluid"`: useful for larger editable forms or media viewers.
* `Sizing="Full"`: full-document experiences such as PDF viewing.

Choose scope first, then sizing.

## Framework Integration

Dialog components integrate with framework state conventions:

* `ModalModel.Show()` sets `Visible=true` and scrolls to modal id.
* `ModalModel.Hide()` clears dismissible alerts and closes modal cleanly.
* Because `ModalModel` inherits `ScreenModel`, modal workflows can use `WithWaiting` and `WithAlerts`.
* `Form<T>` and `Card<T>` automatically use confirmation modals for delete flows.
* `RootModalsCore` keeps viewer modals centralized so any pane can request image/pdf viewing without duplicate modal markup.

## Error Recovery Pattern

For the highest client boundary, prefer mounting the app through `RootRecover<TApp>`:

```csharp
builder.RootComponents.Add<RootRecover<CatalogApp>>("#app");
```

Inside the app itself, place `ErrorRecover` around a subtree only when that subtree should fail independently instead of taking down the whole shell:

```razor
<ErrorRecover>
    <Tabs ... />
</ErrorRecover>
```

On unhandled render exceptions, `ErrorRecover` logs once and shows a fallback error panel. Crudspa doesn't retry rendering automatically at this boundary.

## Practical Guidance

* Keep one model instance per logical modal workflow.
* Use model subclasses for typed modal behavior (`ImageViewerModel`, `PdfViewerModel`, move dialogs, token dialogs).
* Prefer app-root modal hosts for shared global dialogs.
* Avoid ad-hoc raw overlay markup that bypasses model state and waiting/alerts.

## Common Questions

### Should every confirmation be a custom modal?

No. Use built-in confirmation behavior in `Card<T>` and `Form<T>` unless the flow is truly custom.

### How do I close after save and return context?

Call `await Model.Hide()` after successful save. For form modal edit scenarios, use `CancelRequested`/`HideRequested` hooks provided by `Form<T>`.

### When should overlay be disabled?

Only in specialized layouts where modal dimming blocks critical context. Keep overlays enabled in most CRUD flows.

## Tradeoffs

The modal system is intentionally opinionated. You get reliable close behavior and shared waiting/alert UX, but less freedom for one-off overlay mechanics.

The modal's scope is more important than its chrome. Local modals belong to one pane workflow such as edit, confirm, or pick. Global modals belong to shell-wide concerns such as authentication or shared viewers.

## Next Steps

* [Components | Feedback](Feedback.md)
* [Components | Authentication](Authentication.md)
* [Components | Media](Media.md)
* [Documentation Index](../ReadMe.md)

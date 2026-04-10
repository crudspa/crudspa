# Concepts | Models

Blazor makes it easy to put a lot of behavior directly into a component. That's useful at first, but CRUD+SPA screens quickly accumulate waiting states, alerts, filters, edit toggles, modal visibility, and real-time refresh logic. If all of that stays in the component, the view becomes harder to reason about with every new requirement.

Crudspa pushes much of that behavior into client models instead. The goal is simple: let components focus on rendering and composition while models own UI workflow.

## Why This Layer Exists

Crudspa views are meant to stay relatively thin. They bind to a model, inject services, and wire events. The model then tracks the UI state machine that would otherwise sprawl across the component tree.

That's why the model layer is so central to the platform's preferred View, Model, Services approach.

## Core Model Families

| Model Family | Role |
| --- | --- |
| `ScreenModel` | base waiting and alert behavior for most interactive surfaces |
| `ModalModel` | visibility, title, and scroll-aware modal workflow |
| `CardModel<T>` | one item inside a list or find surface, including confirmation state |
| `FormModel<T>` | one editable item inside a `Many` surface |
| `EditModel<T>` | one focused entity editor with new or existing state and read-only toggling |
| `FindModel<TSearch, TEntity>` | search criteria, paging, and result cards |
| `ListModel<T>` | read-heavy collections with replace or remove helpers |
| `ManyModel<T>` | collection editing with add, save, cancel, and delete flows |
| `TabsModel` | tab state tied to the query string so the UI stays deep-linkable |
| move models | focused workflows such as moving panes or segments within the shell |

The point isn't to create a model for every tiny concern. The point is to pull stable UI workflows out of the view layer and give them a clear home.

## What These Models Usually Own

`ScreenModel` is the foundation. It gives a screen a place to put waiting state and structured alerts. That alone removes a surprising amount of noise from Blazor components.

`ModalModel`, `TabsModel`, and the move models then handle common shell and dialog behaviors.

`EditModel<T>`, `FindModel<TSearch, TEntity>`, `ListModel<T>`, and `ManyModel<T>` sit closer to CRUD patterns. They are where read-only toggles, filters, paging, replace or remove helpers, confirmation flows, and collection editing behavior naturally belong.

## Ownership Rules

A good rule of thumb is:

* put rendering and composition in the component
* put UI workflow and temporary screen state in the model
* put feature access in services
* put business rules and trust-boundary logic on the server

That last line is worth keeping in mind. Crudspa models are intentionally rich in UI behavior but poor in business authority. They aren't miniature domain services.

## Practical Guidance

When deciding whether something belongs in a model:

* if it controls waiting, alerts, visibility, sorting, filtering, paging, or edit flow, it's usually model state
* if it changes how markup is arranged, it's usually a view concern
* if it talks to a remote feature boundary, it's usually a service concern
* if it must be trusted, validated, or enforced for all clients, it belongs on the server

This division keeps components small and makes complex screens much easier to maintain.

That same split affects Blazor lifecycle choices. If a component is creating a short-lived model, wiring local handlers, or doing one-time setup, `OnInitialized` / `OnInitializedAsync` is usually the right place. Reserve `OnParametersSet` for cases where the component truly needs to respond to changing parameters during the same instance lifetime.

When a save, cancel, or refresh flow should rebuild local UI state from a fresh entity, prefer remounting the child with `@key` instead of putting one-time initialization back into `OnParametersSet`.

## Tradeoffs

The model layer adds more named types to the client side. For a tiny page, that can feel like extra structure.

But Crudspa isn't optimized for tiny pages. It's optimized for rich, stateful applications. In that world, explicit models are usually much easier to evolve than giant component code-behind files.

## Next Steps

* [Components | Feedback](../Components/Feedback.md)
* [Components | Dialogs](../Components/Dialogs.md)
* [Components | Tabs](../Components/Tabs.md)
* [Documentation Index](../ReadMe.md)

# Components | Overview

This folder is the main reference for building Crudspa user interfaces. Crudspa includes a broad set of reusable components that help teams build beautiful, consistent CRUD+SPA screens without reworking the same forms, lists, dialogs, navigation surfaces, and media patterns in every module.

These docs are organized by workflow and screen shape, not just by single control name. That makes it easier to start from the kind of UI you are building, then drill down into the exact components, models, and integration patterns that support it.

If you want a smaller place to study these patterns in action, start with `Catalog`. Move to `Composer` and `Consumer` when you want to see the same component vocabulary used in broader authoring and runtime surfaces, and use the larger domain modules when you want even more variation.

## Component Shapes

Crudspa's component story has a few distinct layers, and most real screens use more than one of them.

### Lower-Level Controls

`src/Framework/Core/Client/Components` includes the day-to-day building blocks: buttons, textboxes, dropdowns, pickers, layout primitives, menus, alerts, waiting surfaces, uploaders, media viewers, and more. These components keep markup concise while giving common CRUD screens a consistent visual and behavioral baseline.

### CRUD Surfaces

The same component layer also includes higher-level CRUD surfaces such as `Field`, `Form<T>`, `Card<T>`, `List<T>`, `Many<T>`, `Batch<T>`, `Tree`, and `Tabs`. These components capture whole screen patterns instead of just single inputs. They usually pair with models such as `FormModel<T>`, `ListModel<T>`, `ManyModel<T>`, and `TabsModel`, which helps save, cancel, filter, paging, and edit flows feel familiar across modules.

### Plugin Hosts

`src/Framework/Core/Client/Plugins` contains the small Razor-only hosts that load UI by metadata. Components such as `PaneDisplayPlugin`, `PaneDesignPlugin`, `SegmentDisplayPlugin`, and `NavigationDisplayPlugin` dynamically resolve child components at runtime while still enforcing statically typed contracts like `IPaneDisplay` and `ISegmentDisplay`. That combination is a big part of how Crudspa stays extensible without turning the shell into hard-coded branching.

### PaneType Views

Most CRUD+SPA developers spend the bulk of their time in `PaneType` views under `src/*/*/Client/Plugins/PaneType`. These are the actual work surfaces users live in each day: edit panes, list panes, detail panes, and runtime display panes. A `PaneType` view usually composes the lower-level controls, the CRUD surfaces, and the plugin model into one focused feature experience.

A simple mental model is: lower-level controls build sections, CRUD surfaces shape screens, plugin hosts load the right screen, and `PaneType` views are where most feature work comes together.

## Working Style

Crudspa is opt-in, so treat these as default guidance rather than hard rules:

* Start with Crudspa components when a built-in shape fits the problem.
* Prefer one clear default path before introducing many local variants.
* Keep parameters focused on real CRUD decisions instead of exposing every possible vendor option.
* Push UI state and async workflow into models such as `ScreenModel`, `FormModel<T>`, `ListModel<T>`, `ManyModel<T>`, and `ModalModel`.
* Use composition with `Field`, `Form<T>`, `List<T>`, `Many<T>`, and related helpers before reaching for one-off markup.
* Prefer `OnInitialized` / `OnInitializedAsync` for one-time wire-up, model creation, and initial fetches.
* Use `OnParametersSet` / `OnParametersSetAsync` when the same component instance truly needs to react to changing parameters.
* When fresh entity state should rebuild local UI state, prefer remounting with `@key` over replaying initialization logic on every parameter change.

These defaults are what make large portals feel coherent, but you can always step outside them when a specific screen needs something different.

## Page Map

Group | Page | Primary Components
--- | --- | ---
Foundation | [Authentication](Authentication.md) | `SignInEmailTfa`, `SignInEmailTfaModel`, `IAuthService`
Foundation | [Dialogs](Dialogs.md) | `Modal`, `ErrorRecover`, `RootModalsCore`, `ModalModel`
Foundation | [Feedback](Feedback.md) | `Waiter`, `Alerts`, `NoRecords`, `SplashScreen`, `ScreenModel`
Controls | [Textboxes](Textboxes.md) | `TextBox`, `MultilineTextBox`, `SearchTextBox`, `MaskedTextBox`, `NumericTextBox<T>`, `HtmlEditor`, `UserHtml`
Controls | [Dropdowns](Dropdowns.md) | `Dropdown`, `ComboBox`, `IconDropdown`, `EnumDropdown<TEnum>`, `TimeZonePicker`
Controls | [Selections](Selections.md) | `Checkbox`, `CheckedList`, `Radio`, `BoolRadio`, `EnumRadio<TEnum>`, `MultiSelect`
Controls | [Status](Status.md) | `StatusDisplay`, `StatusEdit`, `ContentStatusColors`
Controls | [Pickers](Pickers.md) | `DatePicker`, `DateTimePicker`, `DateFilter`, `ColorPicker`, `FilePicker`
Composition | [Buttons](Buttons.md) | `ButtonCore`, semantic button family, `ButtonGroup`
Composition | [Forms](Forms.md) | `Field`, `Form<T>`, `Card<T>`, `Labeled*`
Composition | [Layouts](Layouts.md) | `Wrapped`, `Stacked`, `Padding`, `Margin`, `Border`, `Toolbar`, `BatchSection`
Composition | [Menus](Menus.md) | `ContextMenu`, `MenuItem`, `BatchMenuItems`
CRUD Surfaces | [Lists](Lists.md) | `List<T>`, `ListOrderables<T>`, `Many<T>`, `ManyOrderables<T>`, `Batch<T>`, `Filters`, `Sorter`, `Pager`
CRUD Surfaces | [Trees](Trees.md) | `Tree`, `TreeNode`, `SegmentTree`, `ConnectingLine`
CRUD Surfaces | [Tabs](Tabs.md) | `Tabs`, `Tab`, `TabsModel`
Domain | [Domain](Domain.md) | `Contact*`, `Organization*`, `UsaPostal*`, `PaneEdit`, `PaneLink`
Media | [Media](Media.md) | uploaders, players, viewers, `RootModalsCore`

## How To Work From These Docs

1. Start with the page that matches the screen shape you are building.
2. Use the option tables to pick the default component path first.
3. Use the integration sections to wire the right model, service, and event patterns.
4. Extend with custom fragments only after the standard composition path is clear.

## Tradeoffs

This layer leans toward consistency and reusable screen shapes because that is what helps larger CRUD+SPA systems age well. That does mean some screens will feel more structured than raw HTML plus ad-hoc controls.

Crudspa is also intentionally opt-in. If a built-in component stops helping, you can drop down to lower-level Razor, direct Blazor markup, or vendor-specific controls without abandoning the rest of the framework.

## Radzen And Crudspa

Crudspa bundles the full Radzen Blazor library alongside its own component layer. In practice, most screens work best when they depend on Crudspa components first, because those components keep parameter surfaces, styling, and CRUD behavior more consistent across panes.

Some Crudspa components are pure framework components. Others are thin wrappers over Radzen with smaller, Crudspa-shaped APIs. When you need a Radzen control that Crudspa does not wrap, you can use it directly.

That layering also helps contain vendor-specific concerns. Screen code can stay mostly Crudspa-shaped while the component layer absorbs more of the direct Radzen dependency.

## Next Steps

* [Components | Forms](Forms.md)
* [Components | Lists](Lists.md)
* [Concepts | Plugins](../Concepts/Plugins.md)
* [Concepts | Models](../Concepts/Models.md)
* [Documentation Index](../ReadMe.md)

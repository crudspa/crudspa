# Components | Domain

Large CRUD systems repeat contact, organization, and postal markup constantly. That duplication creates subtle behavior drift and expensive refactors.

Crudspa provides domain-focused components for common business shapes so teams can focus on rules and workflow instead of rebuilding the same field groups in every module.

## Component Catalog

 Component | Purpose | Primary Integration
 --- | --- | ---
 `ContactEdit` / `ContactDisplay` | Contact editor/display with emails, phones, postals, and optional user account controls | `Contact`, `User`, `BatchModel<T>`
 `OrganizationEdit` / `OrganizationDisplay` | Organization editor/display with role and permission management | `Organization`, `Role`, `Named` permissions
 `UsaPostalEdit` / `UsaPostalDisplay` | US address editor/display with state lookup and zip masking | `UsaPostal`, `IAddressService`
 `PaneLink` | Compact standardized pane navigation row | path routing, count summaries
 `PaneEdit` | Pane type editor with move support and plugin host | `PaneEditModel`, `PaneMoveModel`, pane/segment services

## Default Approach

Use domain components as the first layer in form sections:

```razor
<OrganizationEdit ReadOnly="Model.ReadOnly"
                  Organization="Model.Entity.Organization"
                  PermissionNames="Model.PermissionNames">
    <UsaPostalEdit ReadOnly="Model.ReadOnly"
                   UsaPostal="Model.Entity.UsaPostal" />
</OrganizationEdit>
```

```razor
<ContactEdit ReadOnly="Model.ReadOnly"
             Contact="Model.Entity.Contact"
             User="Model.Entity.User" />
```

This yields stable labels, masks, layout patterns, and batch editing behavior across modules.

## Option Reference

### `ContactEdit`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Contact` | contact aggregate being edited | required
 `User` | optional user account attached to contact | required by component contract, can represent signed-out state
 `ReadOnly` | disables mutation controls | applies across nested batches
 `ChildContent` | extension point between base contact fields and account/batch sections | preferred customization hook

### `OrganizationEdit`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Organization` | organization aggregate | required
 `PermissionNames` | available permissions used when creating roles | role permissions initialized from this list
 `ReadOnly` | disables editing and add/remove role actions | global lock for section
 `ChildContent` | extension point for module-specific fields | inject extra domain fields cleanly

### `UsaPostalEdit`

 Parameter | Purpose | Notes
 --- | --- | ---
 `UsaPostal` | address model | required
 `ReadOnly` | disables all address editing | passed to nested inputs
 `ShowRecipient` | includes recipient field | optional section variant
 `ShowBusiness` | includes business field | optional section variant
 `ChildContent` | top-of-component extension | place contextual intro or helper fields

## What These Components Standardize

* Field sizing and label layout for common business entities.
* Batch editing with add/remove/reorder controls where appropriate.
* Common masking and picker defaults (`(000) 000-0000`, `00000-0000`, time zone, state lookup).
* Consistent empty-state behavior for nested lists.

Consistency here is a major part of keeping developer effort focused on business logic.

## Pane Components

`PaneLink` and `PaneEdit` are framework-level domain helpers:

* `PaneLink` standardizes pane link text and optional count display.
* `PaneEdit` hosts pane type selection and loads the selected pane design plugin.
* `PaneEdit` includes move workflow via `PaneMoveModel` + `Tree`.
* On successful pane move, `PaneEdit` can update route path to the new segment automatically.

Use these components on platform/admin screens instead of custom pane-management markup.

## Framework Integration

Domain components connect directly to framework models and services:

* `ContactEdit` and `OrganizationEdit` use `BatchModel<T>` to manage nested collections.
* `UsaPostalEdit` fetches state names via `IAddressService`.
* `PaneEdit` uses `IPaneService`, `ISegmentService`, `INavigator`, and `IEventBus`.
* `PaneMoveModel` uses `Expandable` trees and modal patterns for safe destination selection.

## Practical Guidance

* Prefer `ChildContent` extension points before forking component markup.
* Keep nested batch entities bound live to the aggregate (`Contact.Emails`, `Organization.Roles`) instead of DTO copies.
* Use `ReadOnly` flow from parent form so domain sections switch mode with the rest of the screen.
* Keep pane editing in platform modules; business modules should consume panes, not re-implement pane administration.

## Common Questions

### Should I customize labels inside these components?

Usually no. Wrap with nearby fields or use `ChildContent` first. Fork only when business vocabulary truly differs.

### Can `UsaPostalEdit` be used outside contact and organization forms?

Yes. It is a standalone address editor and works in any form.

### Why are these not just partial snippets?

Components provide typed contracts, model integration, and consistent behavior that survives refactoring better than copied snippets.

## Tradeoffs

Domain components intentionally reduce micro-layout freedom. In return, teams get a repeatable UI language and lower maintenance across many modules.

## Next Steps

* [Components | Forms](Forms.md)
* [Components | Lists](Lists.md)
* [Components | Trees](Trees.md)
* [Documentation Index](../ReadMe.md)

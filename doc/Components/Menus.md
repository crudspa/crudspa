# Components | Menus

Contextual actions are essential in CRUD screens, but ad-hoc menus often fail in the same ways: inconsistent trigger styles, unreliable close behavior, and uneven icon/text conventions.

Crudspa menu components standardize these concerns with a small, composable API.

## Component Catalog

 Component | Purpose | Primary Integration
 --- | --- | ---
 `ContextMenu` | menu trigger and popup container | `MenuModel`, `IClickService`
 `MenuItem` | single action row with semantic type support | event callback + standardized icon/text
 `BatchMenuItems` | reusable move/remove entries | batch/orderable list actions

## Default Approach

Use semantic menu items where possible:

```razor
<ContextMenu ButtonStyle="ContextMenu.ButtonStyles.Create">
    <MenuItem Type="MenuItem.Types.Reset"
              Clicked="() => Model.Reset()" />
    <MenuItem Type="MenuItem.Types.Refresh"
              Clicked="() => Model.Refresh()" />
</ContextMenu>
```

Use `Type="Custom"` for domain-specific actions:

```razor
<MenuItem Type="MenuItem.Types.Custom"
          Text="Move..."
          Icon="c-icon-move"
          Clicked="() => MoveModel.ShowModal(context.Segment.Id)" />
```

## Option Reference

### `ContextMenu`

 Parameter | Purpose | Options
 --- | --- | ---
 `ChildContent` | menu items content | required
 `ButtonStyle` | trigger intent style | `None`, `User`, `Transparent`, `Create`, `Edit`, `View`
 `Disabled` | disables trigger | `true`, `false`
 `HideBorder` | removes popup border | `true`, `false`

`ButtonStyle.User` renders the user avatar/icon trigger pattern used in auth/account menus.

### `MenuItem`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Clicked` | action callback | required
 `Type` | semantic type (`Custom`, `Reset`, `Refresh`, `Delete`, `Reorder`) | drives default text/icon
 `Text` | custom label for `Type=Custom` | ignored for semantic types
 `Icon` | custom icon for `Type=Custom` | ignored for semantic types
 `StartsGroup` | adds visual group separator | use to separate destructive or secondary actions

### `BatchMenuItems`

 Parameter | Purpose
 --- | ---
 `MoveUpClicked`, `MoveDownClicked`, `MoveLeftClicked`, `MoveRightClicked`, `RemoveClicked` | include only actions relevant to the current row

## Framework Integration

Menus use `MenuModel` internally:

* opening a menu subscribes to `IClickService`
* first click after open is ignored to avoid immediate close
* next outside click closes the menu automatically

This behavior removes a common class of menu-open/menu-close bugs from page code.

## Practical Guidance

* Keep primary actions as buttons; use menu for secondary or overflow actions.
* Prefer semantic `MenuItem` types for consistency (`Refresh`, `Delete`, `Reorder`).
* Use `StartsGroup` before destructive actions when menus contain mixed intent.
* Reuse `BatchMenuItems` in orderable/batch editors instead of repeating move/remove markup.

## Common Questions

### Should delete live in button row or menu?

If delete is a primary action in focused edit context, keep it visible. If row actions are crowded, move delete into a menu.

### Can I mix semantic and custom items?

Yes. This is common in real modules.

### Why use `BatchMenuItems`?

It keeps move/remove wording and iconography consistent across repeated editors.

## Tradeoffs

Centralized menu patterns reduce local trigger/menu styling freedom. In exchange, you get predictable interaction behavior and more uniform CRUD screens.

## Next Steps

* [Components | Buttons](Buttons.md)
* [Components | Lists](Lists.md)
* [Documentation Index](../ReadMe.md)

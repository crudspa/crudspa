# Components | Buttons

Button behavior drifts quickly in CRUD apps. Labels, icon placement, destructive intent, and spacing become inconsistent across modules.

Crudspa prevents this with a compact button family: one flexible base component plus semantic wrappers for common actions.

## Component Catalog

 Component | Use It For | Notes
 --- | --- | ---
 `ButtonCore` | Custom action buttons with standard style vocabulary | Supports text, icon, size, and intent styles
 `ButtonAdd` | Add/create actions | Optional `EntityName`, transparent mode for section headers
 `ButtonSave` | Save action | Consistent save icon and style
 `ButtonCancel` | Cancel action | Consistent cancel styling
 `ButtonEdit` | Edit action | Optional transparent mode
 `ButtonDelete` | Delete action | Standard destructive action style
 `ButtonView` | View/details navigation | Anchor-based action (`Href`)
 `ButtonOk` | Confirmation actions in dialogs | `OkText` override
 `ButtonNote` | Note or log action | Niche semantic button
 `ButtonUp` / `ButtonDown` | Reordering actions | Used by orderable list/form patterns
 `ButtonGroup` | Horizontal action grouping | Optional margin modes

## Default Approach

Use semantic buttons first. Drop to `ButtonCore` only when there is no existing semantic intent.

```razor
<ButtonGroup>
    <ButtonSave Clicked="HandleSaveClicked" />
    <ButtonCancel Clicked="HandleCancelClicked" />
</ButtonGroup>

<ButtonGroup Margin="ButtonGroup.Margins.TinyHor">
    <ButtonCore Clicked="() => Model.CopyJob()"
                IconClass="c-icon-copy"
                Text="Copy" />
    <ButtonCore Clicked="() => Model.RepeatJob()"
                IconClass="c-icon-redo2"
                Text="Repeat" />
</ButtonGroup>
```

## `ButtonCore` Option Reference

 Parameter | Purpose | Default
 --- | --- | ---
 `Clicked` | action callback | required
 `Text` | visible label text | empty
 `IconClass` | icon css class | empty
 `IconPosition` | icon placement (`Left`, `Right`) | `Left`
 `ButtonStyle` | intent style (`Default`, `Save`, `Destroy`, `Transparent`, and others) | `Default`
 `Size` | size scale (`Default`, `Small`, `Large`) | `Default`
 `Disabled` | disables action | `false`

## Semantic Wrapper Options

 Component | Key Parameters | Practical Use
 --- | --- | ---
 `ButtonAdd` | `Text`, `EntityName`, `Transparent`, `Disabled` | inline "Add" in batch section headings
 `ButtonEdit` | `Transparent`, `Disabled` | read-card edit actions
 `ButtonView` | `Href`, `Text` | route to detail/edit page
 `ButtonOk` | `OkText` | modal confirmation labels like "Move", "Apply", "Publish"
 `ButtonGroup` | `Margin` (`None`, `TinyHor`) | consistent spacing between action sets

## Integration Patterns

Buttons are intentionally integrated with higher-level components:

* `Form<T>` and `Card<T>` already include semantic edit/save/cancel/delete actions.
* `ListOrderables<T>` and `ManyOrderables<T>` use `ButtonUp` and `ButtonDown` during reorder mode.
* `Modal` button wells pair naturally with `ButtonOk` + `ButtonCancel`.
* `ContextMenu` can host overflow actions when rows are crowded.

Use these integrations first to reduce custom action wiring.

## Practical Guidance

* Use `ButtonStyle.Transparent` for low-emphasis utility actions, not primary workflow actions.
* Keep destructive actions visually obvious (`ButtonDelete` or `ButtonStyle.Destroy`).
* Use icon-only buttons only when context is clear and repeated across the app.
* Keep navigation intent (`ButtonView`) separate from mutation intent (`ButtonSave`, `ButtonDelete`).

## Common Questions

### When should I not use `ButtonCore`?

When a semantic button already exists. Semantic wrappers keep screens consistent and make future framework changes safer.

### Should I use menu items instead of many buttons?

If actions are secondary or numerous, move them into `ContextMenu`. Keep primary actions directly visible.

### How should I align multiple action groups?

Use multiple `ButtonGroup` instances and spacing wrappers (`Padding` or `Margin`) instead of custom CSS per page.

## Tradeoffs

The button API is intentionally constrained. You lose some local styling freedom, but gain a consistent action language across all CRUD surfaces.

## Next Steps

* [Components | Forms](Forms.md)
* [Components | Menus](Menus.md)
* [Components | Lists](Lists.md)
* [Documentation Index](../ReadMe.md)

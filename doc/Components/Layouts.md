# Components | Layouts

Layout drift is a common source of long-term UI inconsistency in CRUD systems. Ad-hoc CSS choices multiply quickly and make refactoring expensive.

Crudspa layout components provide a shared spacing and alignment vocabulary so form, list, and modal screens stay predictable.

## Component Catalog

 Component | Purpose | Key Options
 --- | --- | ---
 `Wrapped` | horizontal flex wrapper with wrap/no-wrap and alignment modes | `NoWrap`, `Alignment`, `Star`
 `Stacked` | vertical stack with alignment modes | `Alignment`
 `Padding` | semantic inner spacing wrapper | `Size`, `Direction`
 `Margin` | semantic outer spacing wrapper | `Size`, `Direction`
 `Border` | semantic border wrapper | `Direction`
 `Toolbar` | left/right action strip with border modes | `Left`, `Right`, `Border`
 `BatchSection` | titled section shell for nested collections | `Title`, `HeadingPanel`, `ChildContent`

## Default Approach

Compose screens from layout wrappers instead of page-specific CSS utilities:

```razor
<Toolbar>
    <Left>
        <ButtonGroup>
            <ButtonSave Clicked="() => Model.Save()" />
            <ButtonCancel Clicked="HandleCancelClicked" />
        </ButtonGroup>
    </Left>
</Toolbar>

<Padding Size="Padding.Sizes.Small"
         Direction="Padding.Directions.Vertical">
    <Wrapped Alignment="Wrapped.Alignments.Top">
        ...
    </Wrapped>
</Padding>
```

## Option Reference

### `Wrapped`

 Parameter | Purpose | Options
 --- | --- | ---
 `NoWrap` | disables wrapping | `true`, `false`
 `Alignment` | cross-axis/content alignment | `Default`, `Top`, `Center`, `Bottom`, `Right`
 `Star` | stretches wrapper to available width | `true`, `false`

### `Stacked`

 Parameter | Purpose | Options
 --- | --- | ---
 `Alignment` | stack alignment mode | `Default`, `Center`, `Right`, `SelfTop`

### `Padding` and `Margin`

 Parameter | Purpose | Options
 --- | --- | ---
 `Size` | spacing scale | `Tight`, `Micro`, `Tiny`, `Small`, `Medium`, `Large`
 `Direction` | spacing direction | `All`, `Horizontal`, `Vertical`, `Top`, `Right`, `Bottom`, `Left`

### `Border`

 Parameter | Purpose | Options
 --- | --- | ---
 `Direction` | border placement | `All`, `Horizontal`, `Vertical`, `Top`, `Right`, `Bottom`, `Left`, `None`

### `Toolbar`

 Parameter | Purpose | Options
 --- | --- | ---
 `Left` | left content region | render fragment
 `Right` | right content region | render fragment
 `Border` | toolbar border mode | `TopBottom`, `Bottom`, `None`

## Integration Patterns

* Use `Toolbar` for primary actions on list and edit surfaces.
* Use `Wrapped` with `Field` sizing for responsive multi-column form rows.
* Use `BatchSection` for nested collections (`emails`, `phones`, `roles`, and similar groups).
* Keep spacing decisions in wrappers (`Padding`, `Margin`) instead of per-control inline styles.

These patterns make screen intent clearer and reduce custom CSS maintenance.

## Practical Guidance

* Use `Padding` for inside spacing, `Margin` for outside spacing.
* Use `NoWrap` only where control groups must remain on one line.
* Use `Toolbar.Borders.None` in nested modal/edit blocks that already have clear visual containers.
* Keep layout wrappers shallow and readable; avoid deeply nested wrappers without clear purpose.

## Common Questions

### Should I use raw CSS classes instead?

Only when a layout is truly one-off. Prefer layout components for common CRUD structures.

### Why so many size enums?

The enum vocabulary maps to framework CSS classes and gives teams consistent spacing/width language without inventing new values per page.

### When should I use `BatchSection`?

Whenever you render a titled nested collection with heading actions. It keeps repeated patterns like "Email", "Phone", and "Roles" uniform.

## Tradeoffs

Layout components add a thin abstraction over CSS classes. That abstraction is intentional and improves consistency, readability, and refactor safety in large apps.

## Next Steps

* [Components | Forms](Forms.md)
* [Components | Buttons](Buttons.md)
* [Styling | Layouts](../Styling/Layouts.md)
* [Documentation Index](../ReadMe.md)

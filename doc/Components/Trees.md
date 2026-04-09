# Components | Trees

Hierarchical selection appears in many platform workflows: segment moves, pane moves, and tree-based navigation choices. These flows require reliable selected-node and expand/collapse behavior.

Crudspa tree components provide a focused stack built on `Expandable` nodes.

## Component Catalog

 Component | Purpose | Data Shape
 --- | --- | ---
 `Tree` | root tree renderer and selected-id coordination | `ObservableCollection<Expandable>`
 `TreeNode` | recursive node renderer | `Expandable`
 `SegmentTree` | convenience wrapper around `Tree` with prompt text | `ObservableCollection<Expandable>`
 `ConnectingLine` | JS-assisted line between two DOM elements | source and target element ids

## Default Approach

Use `Tree` with `@bind-SelectedId` inside modal workflows:

```razor
<Modal Model="MoveModel"
       Width="26em"
       TopMargin="1em"
       Title="Move Pane">
    <Content>
        @if (MoveModel.Portals.HasItems())
        {
            <Tree Items="MoveModel.Portals"
                  @bind-SelectedId="MoveModel.MoveToId" />
        }
    </Content>
    <Buttons>
        <ButtonOk OkText="Move"
                  Clicked="() => HandleMoveClicked()" />
        <ButtonCancel Clicked="() => MoveModel.Hide()" />
    </Buttons>
</Modal>
```

## Option Reference

### `Tree`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Items` | root node collection | required
 `SelectedId` / `SelectedIdChanged` | selected node id binding | supports `@bind-SelectedId`

Selection propagates through the full collection using `Expandable.Select(id)`.

### `TreeNode`

 Parameter | Purpose
 --- | ---
 `Node` | current node
 `SelectionChanged` | node selection callback propagated up tree

### `SegmentTree`

 Parameter | Purpose | Default
 --- | --- | ---
 `Prompt` | helper text above tree | `"Select the destination segment:"`
 `Items` | node collection | empty
 `SelectedId` / `SelectedIdChanged` | selected node id binding | optional

### `ConnectingLine`

 Parameter | Purpose
 --- | ---
 `SourceId` | source element id
 `TargetId` | target element id

## `Expandable` Integration

Tree behavior relies on `Expandable` and extension helpers:

* `Toggle()` handles expand/collapse with child checks.
* `Select(id)` marks selected node and clears others.
* `ExpandSelected()` opens parent chain for selected node.
* `HasChild(id)` supports move destination validation.

These helpers keep tree semantics consistent across all tree workflows.

## Framework Integration

Common integrations in framework screens:

* `PaneMoveModel` and `SegmentMoveModel` fetch trees from `ISegmentService` and bind to `Tree`.
* modal workflows use `ModalModel` + `Tree` for destination pick and confirmation actions.
* `ConnectingLine` uses `IJsBridge` resize listeners for dynamic visual relationships.

## Practical Guidance

* Keep node ids stable through render cycles.
* Pre-select and expand relevant parent paths before showing move dialogs.
* Use `SegmentTree` when you only need standard prompt + tree behavior.
* Use `ConnectingLine` only when source and target element ids are reliably present.

## Common Questions

### Do I need to manage expanded state manually?

Usually no. `Expandable` helpers handle most expand/selection behavior.

### Can I use `Tree` for non-segment hierarchies?

Yes. Any `Expandable` collection works.

### When should I avoid `ConnectingLine`?

Avoid it when DOM ids are unstable or heavily virtualized. It depends on direct element id targeting.

## Tradeoffs

The tree stack is intentionally minimal. You may need extension for virtualization or drag-and-drop, but baseline selection and hierarchy behavior stays clean and predictable.

## Next Steps

* [Components | Domain](Domain.md)
* [Components | Tabs](Tabs.md)
* [Components | Lists](Lists.md)
* [Documentation Index](../ReadMe.md)

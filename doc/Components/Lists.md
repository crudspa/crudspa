# Components | Lists

List and many-edit surfaces are the core of CRUD productivity. If each module implements filtering, cards, paging, and reorder behavior differently, maintenance cost grows quickly.

Crudspa provides a model-driven list stack so list behavior is consistent across modules while still allowing focused customization.

Most of this stack is Crudspa-native because the real value here is model orchestration, routing, and action semantics. The main vendor-backed piece is `Pager`, which is a thin wrapper over Radzen's pager control.

## Component Catalog

 Component | Purpose | Backing Model | Implementation
 --- | --- | --- | ---
 `List<T>` | read-oriented card list with add/view/delete support | `ListModel<T>` | Crudspa card/list shell
 `ListOrderables<T>` | read list with explicit reorder mode | `ListOrderablesModel<T>` | Crudspa list shell with reorder actions
 `Many<T>` | repeated read/edit forms for child entities | `ManyModel<T>` | Crudspa repeated form shell
 `ManyOrderables<T>` | repeated forms with reorder mode | `ManyOrderablesModel<T>` | Crudspa repeated form shell with reorder actions
 `Batch<T>` | lightweight repeated in-form editor for ordered collections | `BatchModel<T>` | Crudspa repeated editor shell
 `Filters` | standard filter-row shell with field and button regions | render fragments | Crudspa composition shell
 `Sorter` | sort field and direction selector | selected sort and ascending callbacks | Crudspa sort selector
 `Pager` | page navigation for paged results | `Paged` | Radzen `RadzenPager` wrapper
 `BatchMenuItems` | reusable move/remove menu entries | event callbacks | Crudspa menu-item helpers

## Default Approach

Use `List<T>` or `ListOrderables<T>` for read-heavy entity lists:

```razor
<ListOrderables Model="Model"
                T="ForumModel"
                EntityName="Forum"
                UrlPrefix="@($"{Path}/forum")"
                DeleteRequested="id => Model.Delete(id)"
                MoveUpRequested="id => Model.MoveUp(id)"
                MoveDownRequested="id => Model.MoveDown(id)">
    <ReadView>
        <Field Size="Field.Sizes.Small">
            <Image ImageFile="context.Forum.ImageFile"
                   Width="160" />
        </Field>
        <Field Size="Field.Sizes.Full">
            <LabeledHtml Label="Description"
                         HtmlValue="@context.Forum.Description" />
        </Field>
    </ReadView>
</ListOrderables>
```

Use `Many<T>` or `ManyOrderables<T>` when each row needs full edit mode:

* nested child entities
* repeated in-place editors
* item-level save/cancel/delete lifecycle

## Option Reference

### `List<T>` and `ListOrderables<T>`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Model` | list model state and operations | required
 `ReadView` | card content for each entity | required
 `EntityName` | action and empty-state text | required
 `UrlPrefix` | base route for view/new navigation | required for `ButtonView` and add route
 `UrlSuffix` (`List<T>`) | optional query suffix for view/add routes | append custom query state
 `UrlParams` (`ListOrderables<T>`) | query suffix appended to add route | used with reorderable lists
 `ShowFilter` | enables top filter textbox | defaults `false`
 `ShowAdd`, `ShowDelete`, `ShowView` | toggles standard actions | defaults `true`
 `PaneLinks`, `ToolbarMenuItems`, `CardMenuItems`, `Modals` | composition extension points | optional
 `DeleteRequested`, `MoveUpRequested`, `MoveDownRequested` | action callbacks | required for mutation/reorder flows

### `Many<T>` and `ManyOrderables<T>`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Model` | many-edit model | required
 `ReadView`, `EditView` | read and edit fragments per entity | required
 `EntityName` | heading/empty-state text | required
 `ShowFilter` | enables filter textbox | defaults `false`
 `SupportsCreate`, `SupportsDelete` (`Many<T>`) | toggles add/delete | defaults `true`
 `ModalEdit` | edit in local modal instead of inline | useful for dense screens
 `ReadViewContainer` | card read wrapper mode | passes into `Form<T>`
 `Tight` (`ManyOrderables<T>`) | compact card style | useful in nested sections
 `Children` (`ManyOrderables<T>`) | extra content after non-new forms | nested relationship display
 `SaveRequested`, `CancelRequested`, `DeleteRequested`, `MoveUpRequested`, `MoveDownRequested` | callbacks | operation hooks

### `Batch<T>`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Model` | ordered entity collection | required
 `EditView` | row editor fragment | required
 `ReadOnly` | disables remove/reorder | defaults `false`
 `NoRecordsText` | empty-state text | defaults `"None"`
 `AllowReordering` | menu vs remove-only behavior | defaults `true`

## Filtering, Sorting, Paging Composition

Use `Filters`, `SearchTextBox`, `DateFilter`, `Sorter`, and `Pager` together for consistent find/list UX:

```razor
<Filters>
    <Fields>
        <Field Label="Search"
               Size="Field.Sizes.Medium">
            <SearchTextBox @bind-Value="Model.Search.Text" />
        </Field>
        <Field Label="Added"
               Size="Field.Sizes.Medium">
            <DateFilter Range="Model.Search.AddedRange" />
        </Field>
        <Sorter Sorts="Model.Sorts"
                @bind-Selected="Model.Search.Sort.Field"
                @bind-Ascending="Model.Search.Sort.Ascending" />
    </Fields>
    <Buttons>
        <ButtonAdd EntityName="Job"
                   Clicked="AddNew" />
        <ContextMenu ButtonStyle="ContextMenu.ButtonStyles.Create">
            <MenuItem Type="MenuItem.Types.Reset"
                      Clicked="() => Model.Reset()" />
            <MenuItem Type="MenuItem.Types.Refresh"
                      Clicked="() => Model.Refresh()" />
        </ContextMenu>
    </Buttons>
</Filters>
```

## Model Integration

The list stack depends on model contracts, not ad-hoc page state:

* `ListModel<T>` and `ManyModel<T>` derive from `ScreenModel` and include `WithWaiting` and `WithAlerts`.
* `ListOrderablesModel<T>` and `ManyOrderablesModel<T>` add `Reordering`, `MoveUp`, `MoveDown`, `SaveOrder`.
* `SetCards` and `SetForms` normalize sort and apply filter in one flow.
* Components subscribe to model `PropertyChanged` and refresh automatically.

For custom models, override only what you need:

* `Refresh`, `Fetch`, `Remove` are required.
* `Matches`, `OrderBy`, `ApplyFilter`, `InScope` are optional extension points.

## Route And Navigation Behavior

`List<T>` and `ListOrderables<T>` generate add-new routes automatically:

* new id segment is created with `Guid.NewGuid()`
* route appends `state=new`
* optional suffix/params can preserve additional query state

This keeps list pages and edit initialization conventions consistent.

## Practical Guidance

* Default to `List<T>` for read-centric pages, then move to `Many<T>` when inline editing is needed.
* Use orderable variants only where ordinal is an actual business concept.
* Keep filter logic in model overrides, not in component markup.
* Use `Batch<T>` for lightweight nested item editing inside a larger form.

## Common Questions

### Should I build custom list pages directly with `Card<T>`?

Only for unusual cases. Start with `List<T>` or `Many<T>` to preserve shared behavior.

### When do I choose `Many<T>` over `Batch<T>`?

Use `Many<T>` for full read/edit lifecycle per row. Use `Batch<T>` for compact repeated editors without separate read mode.

### How should I handle reordering persistence?

Use orderable model `SaveOrder()` and keep reorder mode explicit with save/cancel actions.

## Tradeoffs

The list stack assumes framework interfaces (`INamed`, `IObservable`, and `IOrderable` where needed). This contract is intentional and enables a very consistent CRUD surface with far less page-specific code, while keeping the only vendor-backed piece narrow and replaceable.

## Next Steps

* [Components | Forms](Forms.md)
* [Components | Menus](Menus.md)
* [Patterns | List](../Patterns/List.md)
* [Documentation Index](../ReadMe.md)

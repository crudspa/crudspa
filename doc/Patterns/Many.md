# Patterns | Many

`Many` is the repeated-row edit pattern. Use it when one pane needs to maintain multiple related records while keeping each row's save, cancel, remove, and reorder behavior explicit.

It works well because the parent scope is clear, row lifecycle methods are explicit, and the model can update only the affected rows when events arrive.

## Canonical Terms

| Term | Meaning |
| --- | --- |
| Node | The feature boundary that owns the row DTOs and row lifecycle methods |
| Batch | An ordered child collection that a row or parent save flow persists together |
| Predicate | Scope constraints that keep row operations in the correct parent and tenancy boundary |

## Flow

1. The pane fetches row DTOs by parent scope and builds `FormModel<T>` rows.
2. Row actions call `Add`, `Save`, `Remove`, or reorder-specific methods.
3. The hub applies permission wrappers and emits row events with scope ids.
4. Other panes update only rows that are in scope.
5. Rows can still carry nested child collections when the repeated surface needs them.

## Contract

`Many` usually needs collection fetch plus per-row lifecycle methods.

```csharp
public interface ISectionService
{
    Task<Response<IList<Section>>> FetchForPage(Request<Page> request);
    Task<Response<Section?>> Fetch(Request<Section> request);
    Task<Response<Section?>> Add(Request<Section> request);
    Task<Response> Save(Request<Section> request);
    Task<Response> Remove(Request<Section> request);
    Task<Response> SaveOrder(Request<IList<Section>> request);
}
```

## Example: Section Many For Page

The model rebuilds row forms from one page-scoped fetch.

```csharp
public override async Task Refresh(Boolean resetAlerts = true)
{
    var request = new Request<Page>(new() { Id = _pageId });
    var response = await WithWaiting("Fetching...", () => _sectionService.FetchForPage(request), resetAlerts);

    if (response.Ok)
        SetForms(response.Value.Select(x => new SectionEditModel(x, _sectionService, ScrollService, _itemService, ElementTypes)));
}
```

Scope-aware event handling prevents cross-page updates.

```csharp
public async Task Handle(SectionAdded payload) => await Replace(payload.Id, payload.PageId);
public async Task Handle(SectionSaved payload) => await Replace(payload.Id, payload.PageId);
public async Task Handle(SectionRemoved payload) => await Rid(payload.Id, payload.PageId);

public override Boolean InScope(Guid? scopeId)
{
    return scopeId is null || scopeId.Equals(_pageId);
}
```

Each row prepares nested element state before save.

```csharp
public override async Task<Response> Save(FormModel<SectionEditModel> form)
{
    var sectionModel = form.Entity;
    var section = sectionModel.Section;

    SetElementsFromModels(form, section);

    return await _sectionService.Save(new(section));
}

private static void SetElementsFromModels(ICardModel<SectionEditModel> form, Section section)
{
    section.Elements.Clear();

    foreach (var elementModel in form.Entity.ElementEditBatchModel.Entities)
    {
        ((IElementDesign)elementModel.DesignComponent.Instance!).PrepareForSave();
        section.Elements.Add(elementModel.Element);
    }
}
```

The hub publishes row updates and the related page content event after a successful save.

```csharp
public async Task<Response> SectionSave(Request<Section> request)
{
    return await HubWrappers.RequirePermission(request, PermissionIds.Pages, async session =>
    {
        var response = await SectionService.Save(request);

        if (response.Ok)
        {
            await Notify(request.SessionId, PermissionIds.Pages, new SectionSaved
            {
                Id = request.Value.Id,
                PageId = request.Value.PageId,
            });

            await GatewayService.Publish(new PageContentChanged { Id = request.Value.PageId });
        }

        return response;
    });
}
```

Reordering stays set-based in SQL.

```sql
update section
set
    section.Ordinal = orderable.Ordinal
    ,section.Updated = @now
    ,section.UpdatedBy = @SessionId
from [Content].[Section] section
    inner join @Orderables orderable on orderable.Id = section.Id
where section.Ordinal != orderable.Ordinal
```

## Pressure Points

### Parent Scope Safety

Many pages are almost always scoped. Include scope ids in events and enforce scope checks before applying row updates.

### Nested Row Models

Repeated rows can still be rich. `SectionEditModel` owns a `BoxModel` plus an `ElementEditBatchModel`, so each row carries its own internal structure without forcing the whole pane back into a single `Edit` surface.

### Order Versus Row Save

Keep row save and row reorder as separate actions. A row save mutates one record. A reorder save mutates the collection's ordinals.

### Batch Inside Row Versus Standalone Many

`SectionManyForPage` is a good example of both patterns working together: the pane is `Many`, but each row still owns an ordered child batch of elements. Use `Many` when the repeated rows themselves need an addressable lifecycle.

## Guidance

* Start with explicit row lifecycle methods such as `Create`, `Add`, `Save`, `Cancel`, and `Remove`.
* Prefer scope-aware event updates such as `Replace(id, scopeId)` over global refreshes.
* Keep row DTOs readable enough for repeated layouts.
* Separate reorder persistence from ordinary row save logic.

`Many` works best when the pane reads as batch-oriented without hiding row-level lifecycle. Each row still has its own create, edit, save, cancel, and remove behavior, and reorder remains a separate concern for the batch as a whole.

## Tradeoffs

`Many` is powerful for high-volume editing, but complexity rises quickly with dense rows and relation-heavy fields. Split workflows or switch to focused edit panes when repeated rows become hard to scan.

## Next Steps

* [Patterns | Edit](Edit.md)
* [Types | Relationship](../Types/Relationship.md)
* [Documentation Index](../ReadMe.md)

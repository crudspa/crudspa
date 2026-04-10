# Patterns | List

`List` is the browse-first collection pattern. Use it when users mostly scan records, compare child counts or status, and open focused edit panes for changes.

It works well because the pane can optimize for readable cards, targeted refreshes, and lightweight actions such as remove or reorder without carrying full row edit state.

## Flow

1. The pane fetches all records in scope.
2. The model normalizes cards and any local UI state such as reorder mode.
3. UI actions call dedicated service methods such as remove or save order.
4. The hub publishes add, save, remove, and reorder events.
5. Other list panes react with `Replace`, `Rid`, or `Refresh` to stay current.

## Contract

`List` and `ListOrderables` typically need collection fetch plus targeted item actions.

```csharp
public interface ITrackService
{
    Task<Response<IList<Track>>> FetchForPortal(Request<Portal> request);
    Task<Response<Track?>> Fetch(Request<Track> request);
    Task<Response> Remove(Request<Track> request);
    Task<Response> SaveOrder(Request<IList<Track>> request);
}
```

## Example: Track List For Portal

The list model uses incremental refresh where possible instead of reloading every card on every event.

```csharp
public async Task Handle(TrackAdded payload) => await Replace(payload.Id, payload.PortalId);
public async Task Handle(TrackSaved payload) => await Replace(payload.Id, payload.PortalId);
public async Task Handle(TrackRemoved payload) => await Rid(payload.Id, payload.PortalId);
public async Task Handle(TracksReordered payload) => await Refresh();

public async Task Handle(CourseAdded payload)
{
    if (payload.TrackId is not null && Cards.HasAny(x => x.Entity.Id.Equals(payload.TrackId)))
        await Replace(payload.TrackId);
}
```

Reordering is explicit and persisted as a dedicated action.

```csharp
public override async Task<Response> SaveOrder()
{
    var orderables = Cards.Select(x => x.Entity.Track).ToList();
    return await WithWaiting("Saving...", () => _trackService.SaveOrder(new(orderables)));
}
```

The hub sends one reorder event after successful persistence.

```csharp
public async Task<Response> TrackSaveOrder(Request<IList<Track>> request)
{
    return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
    {
        var response = await TrackService.SaveOrder(request);

        if (response.Ok)
            await Notify(request.SessionId, PermissionIds.Tracks, new TracksReordered
            {
                PortalId = request.Value.First().PortalId,
            });

        return response;
    });
}
```

The list query can include joins and child counts while keeping card contracts simple.

```sql
select
     track.Id
    ,track.PortalId
    ,portal.[Key] as PortalKey
    ,track.Title
    ,track.StatusId
    ,status.Name as StatusName
    ,track.Ordinal
    ,(select count(1) from [Content].[Course-Active] where TrackId = track.Id) as CourseCount
from [Content].[Track-Active] track
    inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    inner join [Framework].[ContentStatus-Active] status on track.StatusId = status.Id
where track.PortalId = @PortalId
    and organization.Id = @organizationId
```

Ordinal persistence uses set-based updates with `OrderedIdList`.

```sql
update track
set
    track.Ordinal = orderable.Ordinal
    ,track.Updated = @now
    ,track.UpdatedBy = @SessionId
from [Content].[Track] track
    inner join @Orderables orderable on orderable.Id = track.Id
where track.Ordinal != orderable.Ordinal
```

## Pressure Points

### Count Freshness

List cards often expose child counts. Those counts can drift as related entities change elsewhere, so `TrackListForPortalModel` replaces an affected card when `CourseAdded` or `CourseRemoved` arrives.

### Reorder Collisions

Reorder mode should stay explicit so users don't accidentally persist partial ordinal changes. Keep reorder actions separate from ordinary navigation and delete actions.

### Scope And Tenancy

Even collection fetches need hard scope checks. `TrackSelectForPortal` scopes by portal and organization, not just by the current page path.

### Collection Extensions

Real systems add list-specific actions such as copy, duplicate, or archive. Keep those as explicit commands that still route through services and hub notifications.

## Guidance

* Default to `List` for read-heavy collections.
* Use `ListOrderables` only when order is a business concept.
* Keep local presentation state in the model, not scattered through component markup.
* Prefer `Replace` and `Rid` before falling back to a full refresh.

When ordering is part of the workflow, keep reorder as an explicit mode rather than mixing it into ordinary browsing. Users should be able to browse, open, and run lightweight actions in normal mode, then switch into a deliberate save-or-cancel ordering workflow when ordinals change.

## Tradeoffs

`List` keeps collection UX efficient and maintainable, but it intentionally avoids inline multi-row editing complexity. Use `Many` when row-level edit state is primary.

## Next Steps

* [Patterns | Many](Many.md)
* [Patterns | Find](Find.md)
* [Components | Lists](../Components/Lists.md)
* [Documentation Index](../ReadMe.md)

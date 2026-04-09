# Patterns | Edit

`Edit` is Crudspa's default record maintenance pattern. Use it when users need to open one record, understand its current state, change it deliberately, and return to that record over time.

It works well because one pane owns one entity lifecycle: fetch or initialize, validate, save or remove, then react to saved or removed events from elsewhere in the system.

## Canonical Terms

| Term | Meaning |
| --- | --- |
| Node | The feature boundary around one root entity and the contracts that maintain it |
| Sibling | An additional entity intentionally saved alongside that root entity |
| Predicate | An explicit scope or tenancy check enforced in service and SQL boundaries |
| Batch | An ordered child collection persisted as part of one parent save flow |

## Flow

1. A pane initializes an `EditModel<T>` and either creates new state or fetches an existing record.
2. The model sends `Request<T>` envelopes through the client proxy.
3. The hub applies session or permission wrappers and forwards the request to the service.
4. The service validates the entity, coordinates related writes, and executes the underlying sprocs or repositories.
5. The hub publishes saved or removed events after successful mutation.
6. Other panes refresh, replace, or close based on those events.

## Contract

Focused edit surfaces usually need this core contract.

```csharp
public interface ITrackService
{
    Task<Response<Track?>> Fetch(Request<Track> request);
    Task<Response<Track?>> Add(Request<Track> request);
    Task<Response> Save(Request<Track> request);
    Task<Response> Remove(Request<Track> request);
}
```

## Example: Track Edit

The model keeps the new-vs-existing branch explicit.

```csharp
public async Task Save()
{
    if (IsNew)
    {
        var response = await WithWaiting("Adding...", () => _trackService.Add(new(Entity!)));

        if (response.Ok)
        {
            _navigator.GoTo($"{_path.Parent()}/track-{response.Value.Id:D}");
            _navigator.Close(_path);
        }
    }
    else
    {
        var response = await WithWaiting("Saving...", () => _trackService.Save(new(Entity!)));

        if (response.Ok)
            ReadOnly = true;
    }
}
```

The hub only publishes a saved event after the write succeeds.

```csharp
public async Task<Response> TrackSave(Request<Track> request)
{
    return await HubWrappers.RequirePermission(request, PermissionIds.Tracks, async session =>
    {
        var response = await TrackService.Save(request);

        if (response.Ok)
            await Notify(request.SessionId, PermissionIds.Tracks, new TrackSaved
            {
                Id = request.Value.Id,
                PortalId = request.Value.PortalId,
            });

        return response;
    });
}
```

The service keeps sanitization and persistence on the server side.

```csharp
public async Task<Response> Save(Request<Track> request)
{
    return await wrappers.Validate(request, async response =>
    {
        var track = request.Value;

        track.Description = htmlSanitizer.Sanitize(track.Description);

        await sqlWrappers.WithConnection(async (connection, transaction) =>
        {
            await TrackUpdate.Execute(connection, transaction, request.SessionId, track);
        });
    });
}
```

## Pressure Points

### Sibling Writes

Some edit panes look like one form but still need coordinated writes across related entities. `AccountSettingsServiceSql` updates the current `User` and `Contact` in one transaction so profile data stays aligned.

```csharp
var existingContact = await contactRepository.Select(Connection, contactId, PortalId) ?? new() { Id = contactId };

UserContactEmailSync.Apply(existingContact, user, existingContact, existingUser);

await sqlWrappers.WithTransaction(async (connection, transaction) =>
{
    await UserUpdateSettings.Execute(connection, transaction, request.SessionId, user);

    await contactRepository.Update(connection, transaction, request.SessionId, existingContact, PortalId);
});
```

### Batches

`Edit` can still own ordered child collections. `ContactRepositorySql` merges email rows as part of one parent save flow, and the same pattern repeats for phones and postal addresses.

```csharp
incoming.Emails.EnsureOrder();

await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
    existing?.Emails ?? [],
    incoming.Emails,
    ContactEmailInsert.Execute,
    ContactEmailUpdate.Execute,
    ContactEmailDelete.Execute);
```

### Predicates

Predicate logic belongs in SQL, not just in pane code. `TrackUpdate` rejects writes outside the current organization boundary before it updates the row.

```sql
declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

if not exists (
    select 1
    from [Content].[Track-Active] track
        inner join [Framework].[Portal-Active] portal on track.PortalId = portal.Id
        inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
    where track.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end
```

### Real-Time Sync

Open edit panes should react to saved and removed events so another client cannot leave the current record stale.

```csharp
public async Task Handle(TrackSaved payload)
{
    if (payload.Id.Equals(_id))
        await Refresh();
}

public Task Handle(TrackRemoved payload)
{
    if (payload.Id.Equals(_id))
        _navigator.Close(_path);

    return Task.CompletedTask;
}
```

The usual `Edit` rhythm is straightforward: load one record, let the user change local model state, send one authoritative save request through the service boundary, write through SQL, and then let saved or removed notices refresh the rest of the shell.

## Tradeoffs

`Edit` is flexible and familiar, but one pane can become too broad if it absorbs every related workflow. Split out supporting panes when validation rules, permissions, or save frequency diverge.

## Next Steps

* [Patterns | Fill](Fill.md)
* [Patterns | Many](Many.md)
* [Documentation Index](../ReadMe.md)

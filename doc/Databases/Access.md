# Databases | Access

In CRUD systems, the database is the shared resource every request eventually depends on. You can add more web servers, but there is still one transactional source of truth that decides latency, correctness, and concurrency behavior.

Our default access style is direct and explicit: `Microsoft.Data.SqlClient`, stored procedures, and small C# sproxy classes. A sproxy is a stored procedure proxy that builds a `SqlCommand`, adds typed parameters, executes the call, and maps the result into models.

We prefer this style because it performs well in real CRUD workloads and keeps important rules visible. Tenancy checks, permission filters, write ordering, and transaction boundaries stay in places where teams can review and tune them directly.

## Access Stack

 Layer | Responsibility | Why it matters
 --- | --- | ---
 Stored procedure or view | defines the SQL contract and data-tier enforcement | behavior stays visible and reviewable
 sproxy class | builds the command and maps rows into models | keeps each call small and explicit
 `SqlCommandEx` helpers | centralize repeated parameter and reader mechanics | reduces boilerplate without hiding behavior
 `ISqlWrappers` | manages connection and transaction scope | lets services compose several SQL calls safely

## Default Flow

Most request paths follow the same shape:

1. A service method decides which procedure or procedures are needed.
2. A sproxy builds a `SqlCommand` and adds typed parameters.
3. SQL performs the read or write, including tenancy and permission checks.
4. The sproxy maps the result into DTOs or helper models and returns.

This is a typical single-row fetch:

```csharp
public static async Task<Portal?> Execute(String connection, Guid? portalId)
{
    await using var command = new SqlCommand();
    command.CommandText = "FrameworkCore.PortalRunSelect";

    command.AddParameter("@Id", portalId);

    return await command.ReadSingle(connection, ReadPortal);
}

private static Portal ReadPortal(SqlDataReader reader)
{
    return new()
    {
        Id = reader.ReadGuid(0),
        Key = reader.ReadString(1),
        Title = reader.ReadString(2),
        SessionsPersist = reader.ReadBoolean(3),
        AllowSignIn = reader.ReadBoolean(4),
        RequireSignIn = reader.ReadBoolean(5),
        NavigationTypeDisplayView = reader.ReadString(6),
    };
}
```

For lookup-style reads, the sproxy can stay even smaller:

```csharp
public static async Task<IList<Named>> Execute(String connection, Guid? portalId)
{
    await using var command = new SqlCommand();
    command.CommandText = "FrameworkCore.PermissionSelectNames";

    command.AddParameter("@PortalId", portalId);

    return await command.ReadNameds(connection);
}
```

## Helper Surface

`SqlCommandEx` centralizes repeated mechanics so sproxies stay small. Common helpers include typed `AddParameter` overloads, `ExecuteBoolean`, `ReadSingle`, `ReadAll`, `ReadNameds`, and `ReadOrderables`.

Output-parameter checks are common in CRUD validation:

```csharp
public static async Task<Boolean> Execute(String connection, Segment segment)
{
    await using var command = new SqlCommand();
    command.CommandText = "FrameworkCore.SegmentKeyIsUnique";

    command.AddParameter("@Id", segment.Id);
    command.AddParameter("@Key", 100, segment.Key);
    command.AddParameter("@PortalId", segment.PortalId);
    command.AddParameter("@ParentId", segment.ParentId);

    return await command.ExecuteBoolean(connection, "@Unique");
}
```

Insert routines often return generated IDs:

```csharp
public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Achievement achievement)
{
    await using var command = new SqlCommand();
    command.CommandText = "ContentDesign.AchievementInsert";

    command.AddParameter("@SessionId", sessionId);
    command.AddParameter("@PortalId", achievement.PortalId);
    command.AddParameter("@Title", 75, achievement.Title);
    command.AddParameter("@Description", achievement.Description);
    command.AddParameter("@ImageId", achievement.ImageFile.Id);

    var output = command.AddOutputParameter("@Id");
    await command.Execute(connection, transaction);
    return (Guid?)output.Value;
}
```

## Mapping

Reader mapping in this framework is ordinal-based (`ReadGuid(0)`, `ReadString(1)`) instead of name-based. That keeps mapping cost low and makes the SQL select list and C# reader code move together.

The tradeoff is straightforward: if select order changes, the mapper must change too. We still accept that cost because the contract is explicit and the busiest paths stay efficient during profiling and refactoring.

## Set-Based Writes

For batch updates, we use SQL Server table-valued parameters.

The shared types define the payload shape:

```sql
create type [Framework].[IdList] as table (
    [Id] [uniqueidentifier] not null primary key clustered
);

create type [Framework].[OrderedIdList] as table (
    [Id] uniqueidentifier not null,
    [Ordinal] int not null
);
```

The sproxy passes the collection directly:

```csharp
public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, IEnumerable<IOrderable> orderables)
{
    await using var command = new SqlCommand();
    command.CommandText = "FrameworkCore.SegmentUpdateOrdinals";

    command.AddParameter("@SessionId", sessionId);
    command.AddParameter("@Orderables", orderables);

    await command.Execute(connection, transaction);
}
```

The procedure can then enforce scope and update rows set-wise:

```sql
create proc [FrameworkCore].[SegmentUpdateOrdinals] (
     @SessionId uniqueidentifier
    ,@Orderables Framework.OrderedIdList readonly
) as

declare @organizationId uniqueidentifier = (
    select top 1 userTable.OrganizationId
    from [Framework].[User-Active] userTable
        inner join [Framework].[Session-Active] session on session.UserId = userTable.Id
    where session.Id = @SessionId
)

update baseTable
set
     baseTable.Ordinal = orderable.Ordinal
    ,baseTable.Updated = sysdatetimeoffset()
    ,baseTable.UpdatedBy = @SessionId
from [Framework].[Segment] baseTable
    inner join @Orderables orderable on orderable.Id = baseTable.Id
    inner join [Framework].[Segment-Active] segment on segment.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where organization.Id = @organizationId
```

## Composition

`ISqlWrappers` lets service methods compose several procedures in one connection or transaction:

```csharp
public interface ISqlWrappers
{
    Task WithConnection(Func<SqlConnection, SqlTransaction?, Task> func);
    Task<T> WithConnection<T>(Func<SqlConnection, SqlTransaction?, Task<T>> func);
    Task WithTransaction(Func<SqlConnection, SqlTransaction, Task> func);
    Task<T> WithTransaction<T>(Func<SqlConnection, SqlTransaction, Task<T>> func);
}
```

This is what a coordinated save flow looks like in practice:

```csharp
await sqlWrappers.WithTransaction(async (connection, transaction) =>
{
    await SegmentUpdateType.Execute(connection, transaction, request.SessionId, segment);

    await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
        existing?.Panes,
        segment.Panes,
        PaneInsert.Execute,
        PaneUpdate.Execute,
        PaneDelete.Execute);
});
```

This is one of the biggest practical advantages of the sproxy style. Small procedure calls remain reusable, while service code can still express one atomic business operation.

A typical database path starts in a server service, passes through transaction or SQL wrappers, calls a repository or sproxy, executes the stored procedure or query, and returns a mapped result. That path is intentionally explicit. Services coordinate the business operation, while repositories and sproxies keep the SQL call shape readable and reusable.

## Enforcement

Tenancy and permission checks are enforced in SQL so every caller gets the same rules.

This write path shows tenancy enforcement:

```sql
update baseTable
set
     Updated = @now
    ,UpdatedBy = @SessionId
    ,Title = @Title
from [Content].[Achievement] baseTable
    inner join [Content].[Achievement-Active] achievement on achievement.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on achievement.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where baseTable.Id = @Id
    and organization.Id = @organizationId

if @@rowcount = 0
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end
```

This read path shows permission filtering:

```sql
insert into #sessionPermissions (PermissionId)
select distinct rolePermission.PermissionId
from [Framework].[UserRole-Active] userRole
    inner join [Framework].[RolePermission-Active] rolePermission on rolePermission.RoleId = userRole.RoleId
where userRole.UserId = @sessionUserId;

select segment.Id, segment.Title
from [Framework].[Segment-Active] segment
where segment.PortalId = @PortalId
    and segment.AllLicenses = 1
    and (
        segment.PermissionId is null
        or exists (
            select 1
            from #sessionPermissions permission
            where permission.PermissionId = segment.PermissionId
        )
    )
```

For deeper context, see [Tenancy](../Concepts/Tenancy.md), [Security](../Concepts/Security.md), and [Sessions](../Concepts/Sessions.md).

## Multi-Result Reads

When a feature needs a parent row plus related children, we usually fetch them in one call with multiple result sets.

```csharp
return await command.ExecuteQuery(connection, async reader =>
{
    if (!await reader.ReadAsync())
        return null;

    segment = ReadSegment(reader);

    await reader.NextResultAsync();

    while (await reader.ReadAsync())
        segment.Panes.Add(ReadPane(reader));

    return segment;
});
```

This reduces chatty database traffic while keeping mapping explicit.

## Tradeoffs

This style asks the team to own SQL and reader mappings directly. If a project wants rapid schema tinkering without explicit SQL contracts, an ORM-heavy approach may feel easier at first.

In exchange, you get predictable performance, clearer review surfaces, and consistent data-tier enforcement on the busiest shared resource in the system.

## Next Steps

* [Databases | Standards](Standards.md)
* [Databases | Lookups](Lookups.md)
* [Patterns | Edit](../Patterns/Edit.md)
* [Documentation Index](../ReadMe.md)

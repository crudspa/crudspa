# Databases | Versioning

A standard row update overwrites prior state. In many CRUD systems, that becomes a problem the first time someone needs to answer, "what did this record look like yesterday?"

Our default versioning pattern keeps history in the same table with a simple chain based on `VersionOf`. It is straightforward to query, works cleanly with soft deletion, and avoids introducing a separate event model for common audit and recovery needs.

## Versioning Pieces

 Piece | Role | Why it matters
 --- | --- | ---
 `VersionOf` column | points every version back to the root row | history stays queryable with a simple key
 `for update` trigger | copies the previous row into history | updates keep prior state automatically
 `-Active` view | returns only the current row | normal reads stay simple

## Default Approach

This is a representative versioned table:

```sql
create table [Framework].[UserRole] (
    [Id] uniqueidentifier not null,
    [VersionOf] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [UserId] uniqueidentifier not null,
    [RoleId] uniqueidentifier not null,
    constraint [PK_Framework_UserRole] primary key clustered ([Id])
);
```

The update trigger copies the old row into history:

```sql
create trigger [Framework].[UserRoleTrigger] on [Framework].[UserRole]
    for update
as

insert [Framework].[UserRole] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,UserId
    ,RoleId
)
select
     newid()
    ,deleted.Id
    ,deleted.Updated
    ,deleted.UpdatedBy
    ,deleted.IsDeleted
    ,deleted.UserId
    ,deleted.RoleId
from deleted
```

The `-Active` view exposes only the current row:

```sql
create view [Framework].[UserRole-Active] as

select userRole.Id as Id
    ,userRole.UserId as UserId
    ,userRole.RoleId as RoleId
from [Framework].[UserRole] userRole
where 1=1
    and userRole.IsDeleted = 0
    and userRole.VersionOf = userRole.Id
```

## Update Lifecycle

The active row always keeps the root `Id`.

1. On insert, `VersionOf` is set to the new `Id`.
2. On update, the root row is updated in place.
3. The trigger copies the old values from `deleted` into a new history row.

The result is one active row plus zero or more historical rows linked by the same logical root.

A versioned record always has one active row whose `Id` and `VersionOf` match. Older rows get new `Id` values but keep the same `VersionOf`, which is how the history chain points back to one logical root.

## Session Tracking

When tables also include audit metadata, `UpdatedBy` stores the acting `SessionId`:

```sql
update userTable
set  Updated = @now
    ,UpdatedBy = @SessionId
where userTable.Id = @Id
```

We intentionally avoid a strict foreign key from `UpdatedBy` to `[Framework].[Session]`. This keeps write paths simpler and lets session cleanup policies stay independent from history retention.

## Point-In-Time Query

Historical reads are explicit SQL, using a time boundary that is clear and reviewable:

```sql
declare @id uniqueidentifier = '00000000-0000-0000-0000-000000000000';
declare @asOf datetimeoffset(7) = '2026-01-01T00:00:00+00:00';

select top 1
     userRole.Id
    ,userRole.VersionOf
    ,userRole.Updated
    ,userRole.UpdatedBy
    ,userRole.UserId
    ,userRole.RoleId
from [Framework].[UserRole] userRole
where userRole.VersionOf = @id
    and userRole.Updated <= @asOf
order by userRole.Updated desc;
```

If the table also uses soft deletion, include an `IsDeleted` condition when you need "active as-of" behavior.

## When To Use It

This pattern works well when rows change over time and previous state has business value. It is usually unnecessary for static lookup tables or pure append-only telemetry streams.

## Tradeoffs

Versioning adds write amplification and long-term table growth. History-friendly tables also require developers to stay disciplined about reading through `-Active` views for normal application behavior.

In return, you get practical rollback and historical visibility without building a separate temporal architecture.

## Next Steps

* [Databases | Auditing](Auditing.md)
* [Databases | Deletion](Deletion.md)
* [Databases | Standards](Standards.md)
* [Documentation Index](../ReadMe.md)

# Databases | Deletion

Hard deletes are one of the easiest ways to create expensive bugs in CRUD applications. Rows disappear, joins break, reports drift, and support teams lose the ability to recover from user mistakes.

Our default pattern is soft deletion. We keep the row, mark it as deleted, and hide it from normal reads. This preserves history and relational integrity while keeping the everyday query path simple.

## Deletion Modes

 Mode | Default Use | Why it works
 --- | --- | ---
 Soft delete with `IsDeleted` | mutable business data | preserves recoverability and keeps related rows intact
 Hard delete | disposable or explicitly purged data | keeps storage under control when history has no value

Most product data belongs in the first row. Hard delete should be the exception, not the habit.

## Default Approach

A typical table includes `IsDeleted` with a default of `0`:

```sql
create table [Content].[AlignContent] (
    [Id] uniqueidentifier not null,
    [IsDeleted] bit default(0) not null,
    [Name] nvarchar(50) not null,
    [Ordinal] int not null,
    constraint [PK_Content_AlignContent] primary key clustered ([Id]),
);
```

Normal reads go through an `-Active` view:

```sql
create view [Content].[AlignContent-Active] as

select alignContent.Id as Id
    ,alignContent.Name as Name
    ,alignContent.Ordinal as Ordinal
from [Content].[AlignContent] alignContent
where 1=1
    and alignContent.IsDeleted = 0
```

Delete procedures mark rows instead of removing them:

```sql
create proc [FrameworkCore].[SessionEnd] (
    @Id uniqueidentifier
) as

declare @now datetimeoffset = sysdatetimeoffset()

update [Framework].[Session]
set  IsDeleted = 1
    ,Ended = @now
where Id = @Id
    and IsDeleted = 0
```

## Why Active Views Help

The `-Active` view is what keeps soft deletion ergonomic. Reads don't need to repeat `IsDeleted = 0` everywhere, and developers naturally stay on the correct path because the default query surface already excludes deleted rows.

This is also why soft deletion fits well with the rest of the framework. Support and recovery flows can still reach the base table when they need to, while most application code stays simple.

## Lifecycle Planning

Soft-deleted rows still consume storage, so long-lived products need archive and purge routines. That's not a flaw in the pattern. It's a lifecycle responsibility that should be planned from the start.

In this framework, soft deletion pairs naturally with [Auditing](Auditing.md) and [Versioning](Versioning.md) so teams get accountability, recoverability, and historical visibility from one consistent model.

## Tradeoffs

Soft deletion adds storage growth and requires discipline around archive or purge operations. It also means some support and reporting queries need to choose explicitly between base tables and `-Active` views.

That cost is usually worth it. Recoverability and relational stability are far more valuable than the small short-term simplicity of hard delete.

## Next Steps

* [Databases | Auditing](Auditing.md)
* [Databases | Versioning](Versioning.md)
* [Databases | Standards](Standards.md)
* [Documentation Index](../ReadMe.md)

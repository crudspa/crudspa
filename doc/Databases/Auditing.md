# Databases | Auditing

Every mature CRUD system eventually gets the same support question: who changed this, and when?

There are many ways to answer that question. Our default pattern is intentionally lightweight. We store row-level audit metadata directly on the table and refresh it on every write. In most business applications, this gives enough accountability without the complexity of a separate event store.

## Default Approach

These are the baseline audit columns:

 Column | Type | Purpose
 --- | --- | ---
 `Updated` | `datetimeoffset(7)` | records when the last write happened
 `UpdatedBy` | `uniqueidentifier` | records the acting `SessionId` for practical traceability

Storing the session identifier gives support and operations a useful answer quickly, without forcing every table into a heavier auditing architecture.

A representative table looks like this:

```sql
create table [Content].[EmailLog] (
    [Id] uniqueidentifier not null,
    [Updated] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [UpdatedBy] uniqueidentifier not null,
    [EmailId] uniqueidentifier not null,
    [RecipientId] uniqueidentifier not null,
    [RecipientEmail] nvarchar(75) not null,
    [Processed] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Status] int default(0) not null,
    [ApiResponse] nvarchar(max) null,
    constraint [PK_Content_EmailLog] primary key clustered ([Id])
);
```

Write procedures set the values at insert and refresh them at update or soft delete:

```sql
create proc [ContentJobs].[EmailLogInsert] (
     @SessionId uniqueidentifier
    ,@EmailId uniqueidentifier
    ,@RecipientId uniqueidentifier
    ,@RecipientEmail nvarchar(75)
    ,@Status int
    ,@ApiResponse nvarchar(max)
) as

declare @Id uniqueidentifier = newid()
declare @now datetimeoffset = sysdatetimeoffset()

insert [Content].[EmailLog] (
     Id
    ,Updated
    ,UpdatedBy
    ,EmailId
    ,RecipientId
    ,RecipientEmail
    ,Processed
    ,Status
    ,ApiResponse
)
values (
     @Id
    ,@now
    ,@SessionId
    ,@EmailId
    ,@RecipientId
    ,@RecipientEmail
    ,@now
    ,@Status
    ,@ApiResponse
)
```

## Operational Value

This metadata immediately improves production support. You can sort recent changes, identify the acting session, and answer most operational questions without building special-purpose diagnostics.

```sql
select
     emailLog.Id
    ,emailLog.Updated
    ,emailLog.UpdatedBy
    ,emailLog.Status
from [Content].[EmailLog] emailLog
order by emailLog.Updated desc;
```

## When To Extend It

For many CRUD products, this is the right default. It's cheap to maintain and very effective in practice.

When you need to reconstruct prior row values over time, pair this pattern with [Versioning](Versioning.md). When a table is truly static or disposable, adding audit metadata may not be worth the extra write cost.

## Tradeoffs

This approach tells you who last touched a row and when. It doesn't, by itself, tell you what the previous values were or preserve every intermediate state.

That's an intentional trade. The baseline stays cheap, widely applicable, and easy to query. More detailed history should be added only where it pays for itself.

## Next Steps

* [Databases | Versioning](Versioning.md)
* [Databases | Deletion](Deletion.md)
* [Concepts | Sessions](../Concepts/Sessions.md)
* [Documentation Index](../ReadMe.md)

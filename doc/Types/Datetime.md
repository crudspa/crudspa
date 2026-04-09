# Types | Datetime

Date and time fields are where many CRUD systems quietly break. The common failures are mixing calendar dates with real timestamps, skipping timezone conversion, and using inclusive end ranges that double-count results.

Crudspa uses a strict split between date-only values and point-in-time values, then carries that split through SQL, contracts, filters, and UI controls.

## Default Approach

Use this shape by default:

* Calendar dates: SQL `date` and C# `DateOnly?`.
* Real moments in time: SQL `datetimeoffset(7)` and C# `DateTimeOffset?`.
* Date range filtering: `DateRange` + `ResolveDates(timeZoneId)` in the sproxy layer.
* Display: convert to session timezone before formatting.

## Nullability Policy

Crudspa keeps most DTO date/time values nullable on purpose:

* `null` means "not set yet" or "not applicable", which is often a real relational state.
* null values are omitted from JSON payloads by default.
* business rules are enforced in validation and server logic, not by trying to encode all rules into DTO type signatures.

## Option Reference

 Scenario | SQL Type | DTO Type | UI Control
 --- | --- | --- | ---
 Availability window dates | `date` | `DateOnly?` | `DatePicker`
 Event timestamps | `datetimeoffset(7)` | `DateTimeOffset?` | `DateTimePicker`
 Relative list filters | `datetimeoffset(7)` params | `DateRange` offsets | `DateFilter`

## SQL And Contracts

Use `date` for calendar windows:

```sql
create table [Education].[Assessment] (
    [AvailableStart] date default(convert(date, sysdatetime())) not null,
    [AvailableEnd] date default(convert(date, sysdatetime())) not null
);
```

Use `datetimeoffset(7)` for actual moments:

```sql
create table [Education].[AssessmentAssignment] (
    [Assigned] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [StartAfter] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [EndBefore] datetimeoffset(7) default(sysdatetimeoffset()) not null,
    [Started] datetimeoffset(7) null,
    [Completed] datetimeoffset(7) null
);
```

Contracts mirror this split:

```csharp
public DateOnly? AvailableStart
{
    get;
    set => SetProperty(ref field, value);
}

public DateTimeOffset? StartAfter
{
    get;
    set => SetProperty(ref field, value);
}
```

## UI Patterns

Start with the simplest date-only control:

```razor
<DatePicker ReadOnly="Model.ReadOnly"
            Format="d"
            @bind-Value="Model.Entity.AvailableStart" />
```

For timestamp editing, use the basic `DateTimePicker`:

```razor
<DateTimePicker ReadOnly="Model.ReadOnly"
                @bind-Value="Model.Entity.NextRun" />
```

For full picker options, see [Components | Pickers](../Components/Pickers.md).

Use `DatePicker` for calendar concepts such as due dates or publish dates. Use `DateTimePicker` for exact moments such as a next run time. Use `DateFilter` plus `DateRange` when the field is really a search window. Choose the smallest truthful precision. If the business rule only cares about the day, do not force users to supply a time.

## Range Filters

`DateFilter` drives a `DateRange` object (`Any`, `InTheLastWeek`, `Custom`, and so on). In the sproxy layer, resolve those choices into concrete offsets:

```csharp
search.AddedRange.ResolveDates(search.TimeZoneId!);

command.AddParameter("@AddedStart", search.AddedRange.StartDateTimeOffset);
command.AddParameter("@AddedEnd", search.AddedRange.EndDateTimeOffset);
```

In SQL, use an exclusive upper bound:

```sql
and (@AddedStart is null or job.Added >= @AddedStart)
and (@AddedEnd is null or job.Added < @AddedEnd)
```

That `< @AddedEnd` pattern prevents boundary overlap between adjacent ranges.

## Timezone Display

When rendering timestamps, convert from stored offset to the active session timezone:

```csharp
@($" posted on {Model.Entity.Posted.ToLocalTime(SessionState.TimeZoneId):g}")
```

## Tradeoffs

Timezone-aware filtering and conversion add code, but they prevent user-visible errors around "today", "this week", and local reporting windows.

The date/date-time split is strict by design. Avoid collapsing everything to one type just for convenience.

## Next Steps

* [Types | Number](Number.md)
* [Components | Pickers](../Components/Pickers.md)
* [Documentation Index](../ReadMe.md)

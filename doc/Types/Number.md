# Types | Number

Numeric fields often look like plain storage, but in production they usually carry real business rules: limits, schedule cadence, scoring, ordering, and thresholds.

Crudspa keeps numeric behavior explicit by pairing each field with clear validation, typed UI entry, and deliberate SQL mapping.

## Default Approach

Most application numbers in this repository use:

* SQL `int` columns,
* nullable DTO integers (`Int32?`) during edit lifecycles,
* explicit validation for ranges and required values,
* `NumericTextBox<T>` for typed input.

## Nullability Policy

Crudspa commonly uses nullable numeric DTO fields for transport flexibility:

* `null` can mean "not chosen yet" during edit flow.
* null values reduce serialized payload size.
* business constraints are enforced in `Validate` and services, not by forcing non-null DTO properties everywhere.

## Option Reference

 Scenario | Recommended Pattern | Why
 --- | --- | ---
 Counts, ordinals, limits | SQL `int` + `Int32?` DTO | clean default for most CRUD numeric fields
 UI numeric entry | `NumericTextBox<T>` | typed entry, less parsing code
 Required ranges | `Validate()` checks | domain rules stay close to domain model
 Relation ordering | `IOrderable.Ordinal` + `OrderedIdList` TVP | set-based reorder updates

## SQL And Contracts

Scheduling and rule tables rely heavily on integer fields:

```sql
create table [Framework].[JobSchedule] (
    [RecurrenceAmount] int not null,
    [Day] int null,
    [Hour] int null,
    [Minute] int null,
    [Second] int null
);
```

```sql
create table [Education].[ActivityType] (
    [MinChoices] int not null,
    [MaxChoices] int not null,
    [MinCorrectChoices] int not null,
    [MaxCorrectChoices] int not null
);
```

Contracts keep these numeric fields nullable while editing:

```csharp
public Int32? RecurrenceAmount
{
    get;
    set => SetProperty(ref field, value);
}

public Int32? Hour
{
    get;
    set => SetProperty(ref field, value);
}
```

## UI Patterns

Start with a basic `NumericTextBox` binding:

```razor
<NumericTextBox ReadOnly="Model.ReadOnly"
                @bind-Value="Model.Entity.RecurrenceAmount" />
```

`NumericTextBox<T>` supports an explicit `Format` parameter when the display format needs to stay explicit.

For full textbox and numeric input options, see [Components | Textboxes](../Components/Textboxes.md).

When a workflow needs several numeric fields, group them in the same reading order as the business rule. A schedule that reads naturally as `every [amount] [interval] at [hour]:[minute]` is easier to enter correctly than a scattered collection of unrelated number boxes.

## Validation Integration

Keep numeric rules in model validation:

```csharp
if (RecurrenceAmount is < 1 or > 60)
    errors.AddError("Recurrence amount must be between 1 and 60 minutes.", nameof(RecurrenceAmount));

if (Hour is < 0 or > 23)
    errors.AddError("Hour must be between 0 and 23.", nameof(Hour));

if (Minute is < 0 or > 59)
    errors.AddError("Minute must be between 0 and 59.", nameof(Minute));
```

This keeps the UI simple and guarantees server-side enforcement.

## Sproxy Mapping

Sproxies pass numeric fields directly:

```csharp
command.AddParameter("@RecurrenceAmount", jobSchedule.RecurrenceAmount);
command.AddParameter("@Hour", jobSchedule.Hour);
command.AddParameter("@Minute", jobSchedule.Minute);
command.AddParameter("@Second", jobSchedule.Second);
```

Read with typed helpers:

```csharp
RecurrenceAmount = reader.ReadInt32(6),
Hour = reader.ReadInt32(11),
Minute = reader.ReadInt32(12),
Second = reader.ReadInt32(13),
```

For relation reordering, `IOrderable.Ordinal` values are sent as `Framework.OrderedIdList` and updated in one set-based statement.

## Tradeoffs

Do not use numbers as unlabeled state flags. If values represent named states, use enums or lookups.

When numbers are real numeric concepts (amount, position, duration, threshold), Crudspa patterns stay compact and highly predictable.

## Next Steps

* [Types | Enum](Enum.md)
* [Types | Datetime](Datetime.md)
* [Documentation Index](../ReadMe.md)

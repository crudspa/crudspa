# Types | Enum

Enums are a strong fit for stable, code-owned states. They become a maintenance risk when teams use them for values that should be data-managed.

Crudspa treats enums as explicit contracts: store them as SQL `int`, bind them with enum-aware components, and keep business rules in validation and services.

## Default Approach

Use enums when:

* the value set is controlled by code,
* changes happen through deploys,
* semantic labels matter more than arbitrary numeric values.

Use lookup tables (`Named`, `Orderable`) when business users need to add, remove, or rename values in data.

## Nullability Policy

Even when enum properties are non-null in DTOs, Crudspa still follows the same broader type philosophy:

* DTOs stay focused on data transport.
* behavior rules live in source code (`Validate`, service checks, sprocs).
* if a state is optional in your domain, model that explicitly instead of hiding optionality.

## Option Reference

 Scenario | Recommended Shape | Why
 --- | --- | ---
 Finite system states | C# `enum` + SQL `int` | compile-time safety and readable code
 User-managed category lists | lookup table + `Guid` FK | data can change without code changes
 Binary modes with explicit labels | `BoolRadio` | clearer than two-value enum for simple toggles
 Complex state transitions | enum + validation rules | state is typed, transitions are enforced in code

## SQL And Contract

Example enum definitions from `JobSchedule`:

```csharp
public enum RecurrenceIntervals
{
    Second,
    Minute,
    Hour,
    Day,
    Week,
    Month,
}

public enum RecurrencePatterns { Simple, SpecificTime }
```

Persist those as `int`:

```sql
create table [Framework].[JobSchedule] (
    [RecurrenceInterval] int default(0) not null,
    [RecurrencePattern] int default(0) not null,
    [DayOfWeek] int default(0) not null
);
```

## UI Patterns

Start with a basic `EnumRadio`:

```razor
<EnumRadio TEnum="JobSchedule.RecurrenceIntervals"
           ReadOnly="Model.ReadOnly"
           @bind-SelectedValue="Model.Entity.RecurrenceInterval" />
```

When space is tighter, use a basic `EnumDropdown`:

```razor
<EnumDropdown TEnum="JobSchedule.DayOfWeeks"
              ReadOnly="Model.ReadOnly"
              @bind-SelectedValue="Model.Entity.DayOfWeek" />
```

Both components render labels from enum member names (`InsertSpaces()`), so `InTheLastMonth` displays as `In the last month`.

For full selection and dropdown options, see [Components | Selections](../Components/Selections.md) and [Components | Dropdowns](../Components/Dropdowns.md).

`EnumRadio<TEnum>` works best when the option set is small and deserves to stay visible. `EnumDropdown<TEnum>` works better when the set is longer or secondary to the rest of the form. The data type stays the same in both cases; the control choice is about scan speed and form density.

## Sproxy Mapping

Write enum values directly:

```csharp
command.AddParameter("@RecurrenceInterval", jobSchedule.RecurrenceInterval);
command.AddParameter("@RecurrencePattern", jobSchedule.RecurrencePattern);
command.AddParameter("@DayOfWeek", jobSchedule.DayOfWeek);
```

Read them with typed helpers:

```csharp
RecurrenceInterval = reader.ReadEnum<JobSchedule.RecurrenceIntervals>(7),
RecurrencePattern = reader.ReadEnum<JobSchedule.RecurrencePatterns>(8),
DayOfWeek = reader.ReadEnum<JobSchedule.DayOfWeeks>(10),
```

`ReadEnum<TEnum>` returns `default` if the SQL value is null, which is useful but should be intentional.

## Integration Notes

Enum choice rarely stands alone. Keep the cross-field rules in validation:

```csharp
if (RecurrenceInterval is RecurrenceIntervals.Minute
    && RecurrenceAmount is < 1 or > 60)
{
    errors.AddError("Recurrence amount must be between 1 and 60 minutes.", nameof(RecurrenceAmount));
}
```

That keeps enum semantics in the type while business constraints remain in source code.

## Tradeoffs

Persisted enums are numeric contracts. Reordering enum members later can silently change meaning for existing rows.

Treat persisted enum values as stable:

* append new members instead of reordering,
* consider explicit numeric assignments when long-term compatibility matters.

## Next Steps

* [Types | Number](Number.md)
* [Types | Boolean](Boolean.md)
* [Documentation Index](../ReadMe.md)

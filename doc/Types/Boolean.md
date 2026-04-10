# Types | Boolean

Boolean fields are easy to underestimate. Most boolean defects in CRUD systems come from unclear defaults, hidden null handling, or UI controls that don't match intent.

Crudspa keeps boolean handling explicit from SQL `bit` columns through nullable DTO fields, sproxy parameter mapping, and purpose-built UI controls.

## Default Approach

Use this baseline unless you have a strong reason not to:

* SQL storage: `bit` columns, usually `not null` with explicit defaults.
* DTO shape: `Boolean?` when the edit lifecycle needs an unset state.
* Save path: coalesce to a deliberate value when SQL is `not null`.
* Read path: `reader.ReadBoolean(...)` to preserve null semantics.
* UI controls: `Checkbox` for independent flags, `BoolRadio` for mode switches.

## Nullability Policy

Crudspa intentionally keeps many DTO booleans nullable. This is a design choice, not an omission:

* `null` is meaningful in relational systems.
* null fields are naturally omitted from serialized payloads.
* business rules belong in code (`Validate`, service logic, sprocs), not in overly restrictive DTO type signatures.

This pattern shows up directly in contracts:

```csharp
public Boolean? TestAccount
{
    get;
    set => SetProperty(ref field, value);
} = false;

public Boolean? Treatment
{
    get;
    set => SetProperty(ref field, value);
} = false;
```

Validation still enforces required semantics when needed:

```csharp
if (!TestAccount.HasValue)
    errors.AddError("Test Account is required.", nameof(TestAccount));

if (!Treatment.HasValue)
    errors.AddError("Treatment is required.", nameof(Treatment));
```

## Option Reference

 Scenario | Recommended Pattern | Why
 --- | --- | ---
 Independent true/false flags | `Checkbox` + nullable `Boolean?` | simple field editing with optional unset state
 Two explicit modes | `BoolRadio` with clear labels | makes mode intent obvious (`All` vs `Specific`)
 SQL write for required `bit` | `value ?? <domain default>` in sproxy | explicit persistence behavior
 Relation toggles (`AllX`) | `BoolRadio` + `CheckedList` | boolean mode controls whether specific selections apply

## SQL And Sproxy

Model booleans as `bit` columns with intentional defaults:

```sql
create table [Education].[UnitLicense] (
    [AllBooks] bit default(1) not null,
    [AllLessons] bit default(1) not null
);
```

Write booleans explicitly in sproxies:

```csharp
command.AddParameter("@AllBooks", unitLicense.AllBooks ?? true);
command.AddParameter("@AllLessons", unitLicense.AllLessons ?? true);
```

Read booleans with typed reader helpers:

```csharp
return new()
{
    TestAccount = reader.ReadBoolean(4),
    Treatment = reader.ReadBoolean(5),
};
```

Under the hood, null boolean values become `DBNull.Value` when you pass nullable booleans:

```csharp
public void AddParameter(String name, Boolean? value) =>
    command.Parameters.Add(new(name, SqlDbType.Bit)).Value = value as Object ?? DBNull.Value;
```

## UI Integration

Start with the simplest checkbox usage:

```razor
<Checkbox Label="Test Account"
          ReadOnly="Model.ReadOnly"
          @bind-Value="Model.Entity.TestAccount" />
```

For mode selection, start with a basic `BoolRadio`:

```razor
<BoolRadio TrueLabel="All"
           FalseLabel="Specific"
           @bind-SelectedValue="context.UnitLicense.AllBooks" />
<CheckedList ReadOnly="context.UnitLicense.AllBooks == true"
             Selectables="context.UnitLicense.Books" />
```

For full boolean and selection control options, see [Components | Selections](../Components/Selections.md).

Use a simple checkbox when the field is one independent yes/no flag. Use a nullable bool when `unknown` is a real state. Use `BoolRadio` when the boolean acts as a mode switch, especially when it changes the meaning of neighboring controls such as `all` versus `specific` selection.

## Tradeoffs

Nullable booleans improve edit flexibility, but they require explicit save behavior.

If the business meaning isn't truly binary, don't force it into a boolean. Use enum or lookup semantics instead.

## Next Steps

* [Types | Enum](Enum.md)
* [Types | Relationship](Relationship.md)
* [Documentation Index](../ReadMe.md)

# Types | Relationship

Relationships are where CRUD models either stay elegant or become brittle. Columns and foreign keys are only the start. The hard part is preserving clarity across edit UX, save semantics, tenancy checks, and synchronization logic.

Crudspa keeps relationship handling explicit with clear shapes, typed relation contracts, and dedicated relation save flows.

## Default Approach

Use one of these core shapes:

* parent-child (`one-to-many`) with child foreign key and optional ordinal,
* junction (`many-to-many`) with one row per association,
* selectable relation sets represented by `Selectable` lists in DTOs.

Avoid generic "save everything" relation handling when relationship rules have separate lifecycle semantics.

## Nullability Policy

Relationship DTOs remain nullable-friendly for practical CRUD workflows:

* relation IDs can be unset while users are editing,
* relation flags like `AllBooks` can start nullable and become explicit on save,
* constraints are enforced in `Validate`, services, and SQL tenancy checks.

## Shape Reference

 Shape | SQL Pattern | Contract Pattern | Common UI
 --- | --- | --- | ---
 One-to-many ordered children | FK + `Ordinal` (`int`) | `IOrderable` child DTOs | `ManyOrderables<T>`
 Many-to-many selections | junction table rows | `ObservableCollection<Selectable>` | `CheckedList`
 All-or-specific relation mode | parent `bit` columns | nullable booleans (`AllX`) + `Selectable` sets | `BoolRadio` + `CheckedList`

## SQL Modeling

Ordered one-to-many:

```sql
create table [Education].[UnitBook] (
    [UnitId] uniqueidentifier not null,
    [BookId] uniqueidentifier not null,
    [Ordinal] int not null,
    constraint [FK_Education_UnitBook_Unit] foreign key ([UnitId]) references [Education].[Unit] ([Id]),
    constraint [FK_Education_UnitBook_Book] foreign key ([BookId]) references [Education].[Book] ([Id])
);
```

Junction relation:

```sql
create table [Education].[UnitLicenseBook] (
    [UnitLicenseId] uniqueidentifier not null,
    [BookId] uniqueidentifier not null,
    constraint [FK_Education_UnitLicenseBook_UnitLicense] foreign key ([UnitLicenseId]) references [Education].[UnitLicense] ([Id]),
    constraint [FK_Education_UnitLicenseBook_Book] foreign key ([BookId]) references [Education].[Book] ([Id])
);
```

TVP types support set-based relation updates:

```sql
create type [Framework].[IdList] as table (
    [Id] [uniqueidentifier] not null primary key clustered
);

create type [Framework].[OrderedIdList] as table (
    [Id] uniqueidentifier not null,
    [Ordinal] int not null
);
```

## Contract And UI Patterns

`Selectable` is the base relation selection shape:

```csharp
public class Selectable : Named
{
    public Guid? RootId { get; set; }
    public Boolean? Selected { get; set; } = false;
}
```

Selection-based relation editing:

```razor
<BoolRadio TrueLabel="All"
           FalseLabel="Specific"
           @bind-SelectedValue="context.UnitLicense.AllBooks" />
<CheckedList ReadOnly="context.UnitLicense.AllBooks == true"
             Selectables="context.UnitLicense.Books" />
```

Ordered relation editing:

```razor
<ManyOrderables Model="Model"
                T="UnitBookModel"
                EntityName="Book"
                SaveRequested="id => Model.Save(id)" />
```

For complete relationship editing options, see [Components | Lists](../Components/Lists.md), [Components | Selections](../Components/Selections.md), and [Components | Dropdowns](../Components/Dropdowns.md).

Match the UI to the relationship shape. A single foreign key usually wants a dropdown or lookup picker. Ordered child rows want `ManyOrderables`. An `all` versus `specific` mode often wants a boolean or radio toggle paired with a selection control. Relationship editing gets clearer when ordered rows, scoped selections, and mode switches look different because they behave differently.

## Relation Save Flows

Crudspa commonly separates scalar saves from relation saves when relation behavior is distinct. Example: `UnitLicenseServiceSql.Save` vs `SaveRelations`.

Relation sproxy writes relation mode booleans and TVP selections explicitly:

```csharp
command.AddParameter("@Id", unitLicense.Id);
command.AddParameter("@AllBooks", unitLicense.AllBooks ?? true);
command.AddParameter("@AllLessons", unitLicense.AllLessons ?? true);
command.AddParameter("@Books", unitLicense.Books);
command.AddParameter("@Lessons", unitLicense.Lessons);
```

SQL performs tenancy checks first, then set-based updates/inserts:

```sql
if not exists (
    select 1
    from [Education].[UnitLicense-Active] unitLicense
        inner join [Education].[Unit-Active] unit on unitLicense.UnitId = unit.Id
        inner join [Framework].[Organization-Active] organization on unit.OwnerId = organization.Id
    where unitLicense.Id = @Id
        and organization.Id = @organizationId
)
begin
    rollback transaction
    raiserror('Tenancy check failed', 16, 1)
    return
end
```

For reordering, `OrderedIdList` drives a single update statement:

```sql
update unitBook
set
    unitBook.Ordinal = orderable.Ordinal,
    unitBook.Updated = @now,
    unitBook.UpdatedBy = @SessionId
from [Education].[UnitBook] unitBook
    inner join @Orderables orderable on orderable.Id = unitBook.Id
where unitBook.Ordinal != orderable.Ordinal
```

## Tradeoffs

Dedicated relation save flows add structure, but they simplify authorization, testing, and long-term maintenance.

Generic relation updates usually look simpler at first and become difficult once tenancy and lifecycle rules diverge.

## Next Steps

* [Patterns | Many](../Patterns/Many.md)
* [Types | Boolean](Boolean.md)
* [Documentation Index](../ReadMe.md)

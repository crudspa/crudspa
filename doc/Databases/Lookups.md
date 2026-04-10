# Databases | Lookups

Lookup data drives most CRUD screens. Every dropdown, radio list, and filter panel depends on small, stable result sets that feel trivial until they become inconsistent across features.

Our pattern is simple: lookups are regular relational data, exposed through small stored procedures, and mapped into small shared models such as `Named` and `Orderable`.

## Shape Reference

 Shape | Contract | Typical Use
 --- | --- | ---
 `Named` | `Id`, `Name` | dropdowns, radio groups, lightweight filters
 `Orderable` | `Id`, `Name`, `Ordinal`, optional `ParentId` | ordered pickers, tree-like selections, sibling batches

These shared shapes keep lookup behavior predictable across modules.

## Default Query Shape

A name-only lookup usually returns the minimal fields the UI needs:

```sql
create proc [FrameworkCore].[PermissionSelectNames] (
    @PortalId uniqueidentifier
) as

select
     permission.Id
    ,permission.Name
from [Framework].[Permission-Active] permission
    inner join [Framework].[PortalPermission-Active] portalPermission on portalPermission.PermissionId = permission.Id
where portalPermission.PortalId = @PortalId
order by permission.Name
```

An ordered lookup adds `Ordinal` and keeps ordering explicit:

```sql
create proc [ContentDesign].[AlignContentSelectOrderables] as

select
    alignContent.Id
    ,alignContent.Name
    ,alignContent.Ordinal
from [Content].[AlignContent-Active] alignContent
order by alignContent.Ordinal
```

Both patterns read from `-Active` views so soft-deleted rows are excluded by default.

## Sproxy Shape

Lookup sproxies stay intentionally small.

This is the common `Named` pattern:

```csharp
public static async Task<IList<Named>> Execute(String connection, Guid? portalId)
{
    await using var command = new SqlCommand();
    command.CommandText = "FrameworkCore.PermissionSelectNames";

    command.AddParameter("@PortalId", portalId);

    return await command.ReadNameds(connection);
}
```

This is the common `Orderable` pattern:

```csharp
public static async Task<IList<Orderable>> Execute(String connection)
{
    await using var command = new SqlCommand();
    command.CommandText = "ContentDesign.AlignContentSelectOrderables";

    return await command.ReadOrderables(connection);
}
```

These methods are easy to read, easy to review, and hard to misuse.

## Scoping Rules

A lookup should only return values valid for the caller's context. In practice, that often means scoping by portal, organization, session, or parent object.

This is where lookup quality often fails in CRUD systems. A generic lookup leaks values across tenant or permission boundaries, and the UI looks fine until a user sees options they shouldn't have. Keeping lookup queries explicit in SQL makes those boundaries visible and reviewable.

## Practical Guidance

Treat lookup tables as product behavior, not scaffolding. Give them clear ordering rules, include only fields needed by the UI contract, and keep procedure names explicit (`SelectNames`, `SelectOrderables`, `SelectRoleNames`).

If a lookup needs a richer shape for one feature, create a feature-specific procedure instead of overloading a generic one.

## Tradeoffs

Explicit lookup procedures mean more small SQL objects. That's additional surface area compared to a single generic query helper.

The payoff is consistency. Each lookup has a clear contract, the scoping rules are visible, and the UI receives only the shape it actually needs.

## Next Steps

* [Databases | Access](Access.md)
* [Components | Dropdowns](../Components/Dropdowns.md)
* [Components | Selections](../Components/Selections.md)
* [Documentation Index](../ReadMe.md)

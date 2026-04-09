# Databases | Standards

One of the first things people notice in Crudspa SQL is that it does not look like "classic" SQL. That is intentional.

We do not think the old convention of writing SQL keywords in uppercase aged well. SQL is case-insensitive. Modern developers already rely on casing to communicate scope, type, and meaning in languages like C#, TypeScript, and JSON. We think SQL gets easier to read when it follows the same basic idea.

Our style gives visual emphasis to the names that carry business meaning: schemas, tables, views, procedures, columns, parameters, and variables. The keywords fade into the background where they belong.

## Core Claim

We think this style is superior for production CRUD work because it makes SQL easier to scan, easier to refactor, and easier to discuss with the rest of a modern .NET codebase.

That is the real goal of these standards. They are not decorative preferences. They are readability and maintenance choices.

## Style Rules

 Element | Convention | Example | Why we do it
 --- | --- | --- | ---
 SQL keywords | all lowercase | `select`, `from`, `where`, `begin transaction` | keywords are structural noise, not the main event
 User-created database objects | proper casing | `[Framework].[UserRole-Active]`, `[Content].[Achievement]` | object names should stand out immediately
 External variables and parameters | proper casing | `@SessionId`, `@PortalId`, `@Orderables` | these values cross a boundary and behave like contract inputs
 Internal variables | camel casing | `@organizationId`, `@now` | local working values should read like implementation details
 Table aliases | easy-to-read camel casing | `userRole`, `organization`, `portal` | aliases should clarify the query, not compress it into noise
 Multi-line lists | leading comma convention | `,UpdatedBy`, `,Title`, `,ImageId` | diffs and ad hoc edits stay easier to scan
 `-Active` view columns | always use explicit column aliases | `achievement.Title as Title` | tooling and search work better when usage is explicit

## Lowercase Keywords

We keep SQL keywords lowercase on purpose.

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

The query reads from the important nouns outward. `Framework`, `UserRole`, `IsDeleted`, and `VersionOf` carry business meaning. `select`, `from`, and `where` are just glue.

This is why we reject uppercase SQL keywords. They visually overemphasize the least interesting part of the query. They make the syntax shout while the domain model whispers.

## Casing Signals Scope

We use casing to communicate different kinds of names inside the same procedure.

This example shows the distinction clearly:

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

declare @now datetimeoffset = sysdatetimeoffset()
```

`@SessionId` and `@Orderables` are contract inputs. They come from outside the procedure, so they use proper casing.

`@organizationId` and `@now` are local implementation details. They live only inside the procedure, so they use camel casing.

That difference is useful. It tells the reader what entered the procedure from the outside world and what was derived inside the procedure body.

## Alias Style

We always use readable camel-cased table aliases.

```sql
update baseTable
set
     baseTable.Ordinal = orderable.Ordinal
    ,baseTable.Updated = @now
    ,baseTable.UpdatedBy = @SessionId
from [Framework].[Segment] baseTable
    inner join @Orderables orderable on orderable.Id = baseTable.Id
    inner join [Framework].[Segment-Active] segment on segment.Id = baseTable.Id
    inner join [Framework].[Portal-Active] portal on segment.PortalId = portal.Id
    inner join [Framework].[Organization-Active] organization on portal.OwnerId = organization.Id
where organization.Id = @organizationId
    and baseTable.Ordinal != orderable.Ordinal
```

We do not like aliases such as `a`, `b`, `t1`, or `x`. They save a few keystrokes and cost a lot of comprehension.

A good alias is short, obvious, and human. `segment`, `portal`, `organization`, and `baseTable` tell the reader what role each source plays without forcing them to decode a legend.

## Leading Commas

We use the leading comma convention for parameter lists, `select` lists, `insert` column lists, `values` lists, and multi-line boolean expressions where it improves scanability.

```sql
insert [Content].[Achievement] (
     Id
    ,VersionOf
    ,Updated
    ,UpdatedBy
    ,IsDeleted
    ,PortalId
    ,Title
    ,Description
    ,ImageId
)
values (
     @Id
    ,@Id
    ,@now
    ,@SessionId
    ,0
    ,@PortalId
    ,@Title
    ,@Description
    ,@ImageId
)
```

This style is pragmatic.

It makes vertical scanning easier because the delimiters line up. It makes ad hoc editing safer because adding, removing, or moving one item usually changes one line instead of two. It also works well with tooling and scripted transformations because each row in the list has a consistent shape.

We do not think trailing commas are a disaster. We simply think leading commas read better in long SQL blocks, and long SQL blocks are where teams spend their time.

## Explicit Aliases In Active Views

Our `-Active` views always alias every selected column, even when the alias text is identical.

```sql
create view [Content].[Achievement-Active] as

select achievement.Id as Id
    ,achievement.PortalId as PortalId
    ,achievement.Title as Title
    ,achievement.Description as Description
    ,achievement.ImageId as ImageId
from [Content].[Achievement] achievement
where 1=1
    and achievement.IsDeleted = 0
    and achievement.VersionOf = achievement.Id
```

This is not busywork. It helps tools and text search find usages reliably, especially when developers are tracing a column through views, procedures, and C# mappings.

It also makes the projection explicit. A view should make it easy to see exactly which columns are being exposed as part of the contract.

## Why This Matters

SQL is read far more often than it is written. Teams inspect it during debugging, reviews, production support, migrations, and performance tuning.

A good style guide should optimize for those moments, not for nostalgia.

Crudspa SQL is designed to read like modern application code:

* important names stand out,
* local details look local,
* contract inputs look like contract inputs,
* aliases help instead of obscure,
* long lists are easy to scan and edit,
* views make exposed columns obvious.

That is why our SQL may look different than what some teams expect, and it is exactly why we keep it this way.

## Tradeoffs

This style is opinionated, and people coming from uppercase-heavy SQL shops may need a short adjustment period.

We think that cost is minor. Once the reader adapts, the queries become calmer, the domain language becomes more visible, and the codebase becomes easier to navigate as a whole.

## IDE Setup

Most SQL editors ship with defaults that do not match Crudspa SQL. The settings below bring each tool closer to the conventions in this page, especially lowercase keywords, leading commas, readable multi-line lists, and consistent indentation.

The menu names below were checked against current vendor docs on March 6, 2026. At that time this meant Visual Studio 2026, ReSharper 2025.3, SSMS 21, Rider 2025.3, and the current Visual Studio Code SQL tooling.

### Visual Studio Community

For this repository, Visual Studio Community 2026 plus SSDT fits the existing `Database.sqlproj` workflow well.

* Install `SQL Server Data Tools` from the Visual Studio Installer under `Data storage and processing`.
* In `Tools > Options > Text Editor > All Languages > Tabs`, choose `Insert spaces`, `Tab Size = 4`, and `Indent Size = 4`.
* Use nearby SQL files in this repository as your formatting model when creating new views and procedures.

### ReSharper

ReSharper adds the SQL formatting controls that matter most for this style.

* Set the solution dialect at `ReSharper > Options > Code Inspection > SQL > SQL Dialects` so `.sql` files are treated as Transact-SQL.
* In `ReSharper > Options > Code Editing > SQL > Formatting Style > Queries`, set `Place comma` to `To begin` for the clause types you use most often.
* Use wrapping options that `Chop` or `Chop if long` so long `select`, `from`, `where`, `group by`, and `order by` lists naturally stay one item per line.
* In the SQL `Case` settings, keep `Word Case` at `Do not change`.
* If your team shares ReSharper settings, save these rules in a team-shared `.DotSettings` layer so cleanup behaves the same on every machine.

### SSMS

SSMS 21 works well for ad hoc queries, debugging, and production support.

* In `Tools > Options > Text Editor > All Languages > Tabs`, choose `Insert spaces`, `Tab Size = 4`, and `Indent Size = 4`.
* In `Tools > Options > Text Editor > Transact-SQL > IntelliSense`, set `Casing for built-in function names` to lowercase.
* After changing IntelliSense options, open a new Transact-SQL editor window before judging the result.

### Visual Studio Code

Visual Studio Code works best here with the current `MSSQL` extension and workspace-level settings.

Start with settings like these in `.vscode/settings.json`:

```json
{
    "[sql]": {
        "editor.insertSpaces": true,
        "editor.tabSize": 4,
        "editor.detectIndentation": false,
        "editor.formatOnSave": true
    },
    "mssql.format.keywordCasing": "lowercase",
    "mssql.format.datatypeCasing": "lowercase",
    "mssql.format.placeCommasBeforeNextStatement": true,
    "mssql.format.placeSelectStatementReferencesOnNewLine": true
}
```

Then finish the setup like this:

* Use `Format Document With...` once and set the MSSQL formatter as the default formatter for SQL files.
* Keep this configuration at the workspace level so everyone opening the repository starts from the same formatter behavior.

### Azure Data Studio

Azure Data Studio uses the same style of SQL settings as Visual Studio Code for this kind of work.

* Open `File > Preferences > Settings` and apply the same SQL formatter values shown above for Visual Studio Code.
* Set spaces with a tab size of `4` for SQL files.
* Set the MSSQL formatter as the default formatter for SQL files.
* Turn on format-on-save for SQL if you want the formatter to keep keywords, datatypes, and comma placement aligned automatically.

### JetBrains Rider

Rider 2025.3 gives you the same helpful SQL style controls in a standalone JetBrains IDE.

* Confirm that the bundled `Database Tools and SQL` plugin is enabled.
* Go to `Settings > Editor > Code Style > SQL`.
* On `Tabs and Indents`, use spaces with `Tab size = 4` and `Indent = 4`.
* On `Queries`, set `Place comma` to `To begin` and use chopping for long `select`, `from`, `with`, `where`, `group by`, and `order by` lists.
* On `Case`, set `Word Case` to `Do not change` and enable `Use original case`.

Across all of these tools, the most helpful settings are the same: 4-space indentation, lowercase SQL keywords where the formatter supports them, leading commas, and one item per line in long lists.

## Next Steps

* [Databases | Access](Access.md)
* [Databases | Lookups](Lookups.md)
* [Databases | Migrations](Migrations.md)
* [Documentation Index](../ReadMe.md)

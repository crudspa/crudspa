# Databases | Migrations

Schema change is where many CRUD projects lose confidence. Teams move quickly at the feature level, then hesitate when a database change might put production data at risk.

Our migration model is based on SQL Server Data Tools (SSDT) and the SQL project at `src/Database/Database.sqlproj`. SSDT compiles the project into a dacpac, and we generate deployment scripts for review before publishing. This keeps change history explicit and reviewable.

We have found SSDT especially valuable during refactoring. The model stays coherent while schemas evolve, and the generated publish scripts make each change visible before it runs.

## Migration Assets

 Asset | Role | Why it matters
 --- | --- | ---
 `Database.sqlproj` | source-controlled definition of the database | schema change becomes a code change
 dacpac output | compiled deployable package | publish behavior is generated from the current source model
 publish profile | environment-specific deployment settings | important safety defaults stay explicit
 generated publish script | final review surface | teams can inspect exactly what will run

## Default Approach

Every object lives in source control as SQL. Tables, views, triggers, types, and stored procedures are all part of the project. A schema change is never just a manual production action.

That discipline matters because database drift is expensive. Once teams start fixing production directly, the real source of truth becomes unclear and future deployments get riskier.

## SSDT Refactors

Two SSDT refactors are especially helpful in day-to-day work.

`Rename` lets you rename tables, columns, procedures, and other objects while preserving dependency awareness across the SQL project. This is much safer than ad-hoc text replacement, especially in larger databases.

`Move to Schema` lets you move an object into a more appropriate schema as responsibilities become clearer. That is useful when an object started in a temporary location and later needs a cleaner long-term home.

Both refactors are reliable in our experience, but the same rule still applies: always review the generated deployment script before publishing.

## Safety Settings

The local publish profile (`src/Database/Deploy-LocalMachine.publish.xml`) shows important safety defaults:

 Setting | Value | Why it matters
 --- | --- | ---
 `BlockOnPossibleDataLoss` | `True` | forces an explicit decision when a change could destroy data
 `DropObjectsNotInSource` | `True` | removes drift so deployed environments match source control
 `DropIndexesNotInSource` | `False` | avoids unnecessary index churn during normal publishes

Together, these settings encourage disciplined migrations, but they also require careful review before applying scripts to shared environments.

## Practical Workflow

1. Change schema objects in the SQL project.
2. Build the project and fix warnings or errors.
3. Generate and review the deployment script.
4. Apply in development and verify application behavior.
5. Promote the same migration through higher environments.

If a change is destructive, use an expand-contract approach when needed: add new structure first, backfill and cut over in application code, then remove old structure in a later release.

## Complex Changes

Some migrations cannot be made safe in one step. When a change touches high-volume tables, shared relationships, or tenancy-sensitive data, break it into phases and rehearse it against realistic data.

A common pattern is: prepare new structure first, deploy the schema change, run targeted backfill or cleanup, then verify behavior before removing old structure. Rehearsing that flow several times is normal for serious migrations.

Treat migration work as a loop rather than a one-time checklist: change the SQL project, build it, generate and review the publish script, test against real data, and then promote the same reviewed change through environments. Serious migrations are usually rehearsed more than once before they are trusted.

## Tradeoffs

SSDT adds process and review discipline. Teams that want totally ad-hoc database editing may find that slower at first.

The payoff is substantial: migrations become repeatable, schema drift stays under control, and risky changes are exposed early enough to fix before deployment.

## Next Steps

* [Databases | Standards](Standards.md)
* [Databases | Versioning](Versioning.md)
* [Overview | Tools](../Overview/Tools.md)
* [Documentation Index](../ReadMe.md)

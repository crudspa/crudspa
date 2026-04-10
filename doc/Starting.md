# Getting Started

This repository combines the reusable Crudspa libraries with focused sample applications. The easiest way to get comfortable is to learn it through the shipped samples first, then read the reusable libraries with that runtime picture already in your head.

The fastest path after cloning is concrete:

* publish the local database from the checked-in SQL project
* seed the sample blobs
* run `Catalog`, then `Composer` plus `Consumer`, then `Samples/Jobs/Engine` when you want background work

Once those flows are real, the rest of the workspace becomes much easier to navigate.

## Prerequisites

To work comfortably in Crudspa, you will want:

* a current .NET SDK that can build the `net10.0` projects in [src/Crudspa.slnx](../src/Crudspa.slnx)
* Visual Studio with SQL Server Data Tools if you want to build or publish [src/Database/Database.sqlproj](../src/Database/Database.sqlproj)
* a local SQL Server instance for schema and data work
* Git and the `dotnet` CLI
* optionally, a larger host solution of your own if you want to compare these libraries and samples with a broader application composition

If your host solution compiles Sass from ordered stylesheet manifests, keep a Sass tool available as well. The framework itself is happy on the standard .NET and SQL Server toolchain, but real hosts usually add a stylesheet step.

If you want the sample scenarios to work exactly as documented, use Visual Studio and keep the checked-in HTTPS launch URLs in the sample server projects.

## Solution Map

[src/Crudspa.slnx](../src/Crudspa.slnx) is the best entry point for this repository. It groups the source into the main library families, plus the sample applications that demonstrate how those families are composed:

* [Framework](../src/Framework) is the foundation. `Core` contains navigation, contracts, wrappers, components, client models, and server boundaries. `Jobs` adds background scheduling and worker support.
* [Content](../src/Content) builds higher-level authoring, display, and content-job features on top of the framework.
* [Education](../src/Education) shows domain-shaped modules built with the same client, shared, and server layering used everywhere else.
* [Database](../src/Database) contains the shared SQL Database Project.
* the sample applications show the framework in progressively richer shapes: `Catalog`, `Composer`, `Consumer`, and the `Jobs Engine`.

There are really two shapes to keep in your head here. The source tree is organized into the reusable families: `Framework`, `Content`, `Education`, and `Database`. The learning path cuts across that tree through the sample track: `Catalog` is the focused `Framework.Core` story, `Composer` and `Consumer` show the two content sides, and `Samples/Jobs/Engine` shows how the jobs stack executes work while the web hosts keep the administration surfaces.

## First Build

Start by proving the managed projects build cleanly:

```powershell
dotnet build src/Crudspa.slnx
```

That validates the reusable client, shared, and server projects. It's the right first check after cloning.

The database follows a different workflow. [src/Database/Database.sqlproj](../src/Database/Database.sqlproj) is a SQL Database Project, so treat it like one: build and publish it with the normal Visual Studio or MSBuild plus SSDT path instead of expecting `dotnet build` to cover everything.

It's also normal to notice that the reusable projects and the sample applications have different responsibilities. The libraries hold the long-lived architecture. Each application adds the final composition, metadata, and branding needed to turn those libraries into a working product.

## Application Shape

Crudspa becomes much easier to reason about once you separate reusable library code from application code.

The libraries give you the shell, component vocabulary, service boundary, session model, and data-layer patterns. Your application supplies the domain-specific panes, metadata, branding, and composition choices that make it your own.

That relationship is important enough to deserve its own page. Read [Overview | Applications](Overview/Applications.md) before you try to infer the whole framework from scattered startup files.

For the host-by-host sample walkthrough after setup, read [Overview | Samples](Overview/Samples.md).

## Local Sample Setup

Before you launch the samples, do this once:

1. Open [src/Crudspa.slnx](../src/Crudspa.slnx) in Visual Studio.
2. Publish [src/Database/Database.sqlproj](../src/Database/Database.sqlproj) to your local SQL Server instance by using [src/Database/Deploy-LocalMachine.publish.xml](../src/Database/Deploy-LocalMachine.publish.xml). That creates or updates the `Crudspa-Local` database used by all of the samples.
3. Run `.\art\SeedSampleBlobs.ps1` from the repo root. Sample media files live under `art/Blobs`, and this script copies them into the local `BlobServiceLocal` store used by the sample hosts.
4. If you plan to sign in to `Composer`, keep an eye on `C:\data\temp\email`. The sample servers use `EmailSenderLocalFile`, so password-reset and access-code messages are written there instead of being sent to a real mailbox.

The checked-in sample configuration assumes these HTTPS launch URLs:

* `Catalog`: `https://localhost:42100`
* `Composer`: `https://localhost:42200`
* `Consumer`: `https://localhost:42300`

Those URLs are also listed in the sample `EventReceiverUrls` settings, so leave them in place if you want the cross-host gateway flows to work without extra setup.

## First Sample Runs

### Catalog

Start with `Catalog` if you want the smallest complete `Framework.Core` example. It gives you one focused CRUD+SPA with a public shopping cart flow, private catalog and order-management panes, and a simplified sign-in experience that accepts any name.

Open `https://localhost:42100`, enter a name, and use that session to trace one real feature from pane, to model, to hub, to service, to SQL. This is still the best first stop after cloning the repo.

### Composer And Consumer

Run `Composer` and `Consumer` together when you want to study `Content.Design` and `Content.Display` as one story instead of two isolated projects.

Open `Composer` at `https://localhost:42200` and sign in with `sample@example.com`. Use the built-in `Reset password` flow, read the access-code email from `C:\data\temp\email`, set a password, and finish signing in. Then keep `Consumer` open at `https://localhost:42300` while you edit pages, sections, styles, or portal content in `Composer`. That's the shortest path to seeing authored changes invalidate caches and appear in the runtime host in real time.

### Jobs Engine

Run [src/Samples/Jobs/Engine/Engine.csproj](../src/Samples/Jobs/Engine/Engine.csproj) alongside `Catalog` or `Composer` when you want to study background work.

The jobs administration UI lives in the web hosts. Both `Catalog` and `Composer` expose `Jobs` and `Schedules` panes. The standalone sample under `src/Samples/Jobs` is the engine host that schedules due work, executes job actions, and publishes job updates back into those panes through the gateway flow.

## First Code Tour

Once you have a successful build or a running host, read in this order:

* [Overview | Architecture](Overview/Architecture.md)
* [Concepts | Navigation](Concepts/Navigation.md)
* [Concepts | Services](Concepts/Services.md)
* [Concepts | Plugins](Concepts/Plugins.md)
* [Concepts | Models](Concepts/Models.md)

Then walk the source in roughly this order:

* [src/Framework/Core/Client/Models](../src/Framework/Core/Client/Models) for the client state layer
* [src/Framework/Core/Client/Plugins](../src/Framework/Core/Client/Plugins) for shell and pane composition
* the sample application's root UI shape
* the sample application's application-specific registration and metadata
* one representative pane plugin in the module you care about

That path gets you from the shell, to the client model layer, to the service boundary, to one real feature.

## Practical Guidance

Crudspa makes more sense if you trace one complete request than if you skim twenty folders. Pick a single pane, follow its model into a client proxy, then into a hub method, a server service, and finally the SQL layer. `Catalog` is intended to make that walk especially short.

When you are studying cross-host behavior, keep the sample split in mind. `Composer` plus `Consumer` shows content invalidation and runtime refresh. `Catalog` or `Composer` plus `Samples/Jobs/Engine` shows jobs scheduling, execution, and UI refresh. Those are some of the most distinctive parts of the platform, and they are easiest to understand when both relevant hosts are running at the same time.

Also, keep the SQL project in view from the start. Crudspa's default workflow treats the data layer as part of the architecture, and the Visual Studio plus SSDT experience is meant to keep that work visible instead of incidental.

## Next Steps

* [Overview | Features](Overview/Features.md)
* [Overview | Samples](Overview/Samples.md)
* [Overview | Applications](Overview/Applications.md)
* [Overview | Libraries](Overview/Libraries.md)
* [Concepts | Navigation](Concepts/Navigation.md)
* [Documentation Index](ReadMe.md)

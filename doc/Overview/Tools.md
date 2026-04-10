# Overview | Tools

Crudspa sits in a part of the .NET world where frontend, backend, SQL Server, and styling all have to work together at the same time. If those tools are misaligned, the day-to-day development experience feels heavier than it should.

The good news is that Crudspa works best with a very practical toolchain. Nothing exotic is required. The main win comes from using the right tool for each layer and keeping those layers visible.

## Required Tools

### .NET SDK And CLI

You need a current .NET SDK that can build the `net10.0` projects in this repository. The `dotnet` CLI is the quickest way to validate the managed projects:

```powershell
dotnet build src/Crudspa.slnx
```

Use the CLI for fast feedback, restore, build validation, and publish steps that belong to the managed project graph.

### Visual Studio And SSDT

Visual Studio with SQL Server Data Tools is strongly recommended. It lets you work on the C# projects and the database project in the same IDE instance, with the normal SQL Database Project experience for [src/Database/Database.sqlproj](../../src/Database/Database.sqlproj), including build, schema comparison, and publish.

### SQL Server

You will want a local SQL Server instance for real development work. Crudspa assumes real relational constraints, stored procedures, and data-layer validation or tenancy checks are part of the application rather than an afterthought. A lightweight mock database setup doesn't teach the right lessons here.

## Recommended Tools

The following aren't always mandatory, but they materially improve day-to-day work:

* a strong C# refactoring environment for navigating large client, shared, and server solutions
* a SQL editor you are comfortable with for inspecting data, procedures, and migration results
* browser developer tools for layout, network, and WebAssembly debugging
* a Sass workflow for hosts that build styles from explicit SCSS stylesheet entrypoints

On the styling side, Crudspa works best when teams keep two things visible:

* the host stylesheet entrypoints that define structural stylesheet composition
* the runtime theme token contract that flows through `defaults.scss`

That split is a good fit for Crudspa because the platform shares a broad style foundation across components, shells, and content surfaces while still allowing portal-specific branding to flow in at runtime.

## Typical Workflow

A healthy Crudspa workflow usually looks like this:

1. Read the docs for the concept you are about to touch.
2. Build the managed projects with `dotnet build`.
3. Use Visual Studio for deeper code navigation, SQL project work, and publish flows.
4. Run the narrowest sample or host, or sample pair, that exercises the behavior you changed.
5. Publish the database deliberately instead of treating it as a side effect of app startup.

When styling is part of the task, a practical loop usually looks like this:

1. Edit the relevant module or host stylesheet entrypoint, or its underlying SCSS.
2. Refresh the sample or host and verify the structural CSS change.
3. If you change `defaults.scss`, verify both the host stylesheet and the runtime theme output.
4. Verify the runtime theme still lands correctly through `Composer` or `Consumer` and, when appropriate, through the style preview UI.

## Practical Guidance

Keep the docs open while you work. In Crudspa, vocabulary and architecture are part of the toolchain. If you skip those, it's easy to misread a `pane`, a `node`, or a runtime content flow.

Also, prefer working from the smallest runnable sample that still exercises the behavior in question. Start with `Catalog` for `Framework.Core` questions, move to `Composer` and `Consumer` for authored runtime or CMS behavior, use `Catalog` or `Composer` plus `Samples/Jobs/Engine` for background-work questions, and reach for the larger domain modules when you need broader composition examples.

For the shipped samples, keep the default HTTPS launch URLs in place unless you intentionally want to reconfigure them. The checked-in sample appsettings point the gateway receiver URLs at `Catalog` on `42100`, `Composer` on `42200`, and `Consumer` on `42300`, so those defaults make the cross-host flows work with no extra editing.

If you want the concrete sample combinations and what each one demonstrates, read [Overview | Samples](Samples.md).

## Tradeoffs

Crudspa is most comfortable in the Microsoft stack. That means the default toolchain is especially pleasant for .NET and SQL Server developers, but less ideal for teams that want to avoid Visual Studio, SSDT, or SQL Server entirely.

We think that trade is worth it. The architecture is simpler and more coherent when the database, services, UI, and styling pipeline are all treated seriously.

## Next Steps

* [Getting Started](../Starting.md)
* [Overview | Samples](Samples.md)
* [Overview | Architecture](Architecture.md)
* [Styling | Stylesheets](../Styling/Stylesheets.md)
* [Databases | Standards](../Databases/Standards.md)
* [Documentation Index](../ReadMe.md)

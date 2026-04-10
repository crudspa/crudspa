![Crudspa](art/Logo-Mark-Colored.svg)

# Crudspa

Crudspa is an MIT-licensed open-source framework for building rich CRUD+SPA applications with C#, Blazor WebAssembly, SignalR, and SQL Server.

The hard part of line-of-business software is rarely the first screen. The hard part is keeping navigation, UI workflow, service boundaries, real-time updates, validation, permissions, and data rules clean as the application grows. Crudspa is designed so those concerns reinforce each other instead of drifting apart.

The result is a platform for deep-linkable portals, responsive admin surfaces, runtime content experiences, background jobs, and focused sample applications that can all share the same contracts, naming, and operational model.

> [!NOTE]
> Looking for the public project overview first? Start at [crudspa.org](https://crudspa.org).
>
> This repository contains the reusable libraries, the shared SQL Database Project, and focused sample applications. The docs in [doc/ReadMe.md](doc/ReadMe.md) are the primary source reference.

## Quick Start

The fastest way to understand this repo is to run the shipped samples in the order they were designed to teach the platform:

1. Open [src/Crudspa.slnx](src/Crudspa.slnx) in Visual Studio.
2. Publish [src/Database/Database.sqlproj](src/Database/Database.sqlproj) to your local SQL Server instance by using [src/Database/Deploy-LocalMachine.publish.xml](src/Database/Deploy-LocalMachine.publish.xml). That creates or updates the `Crudspa-Local` database used by the samples.
3. Run `.\art\SeedSampleBlobs.ps1` from the repo root to seed the sample media files into the local blob store.
4. Start `Catalog` at `https://localhost:42100` and enter any name to simulate signing in. This is the fastest path to the `Framework.Core` shell, model, service, notice, and SQL story.
5. Start `Composer` and `Consumer` together at `https://localhost:42200` and `https://localhost:42300`. In `Composer`, use `sample@example.com`, choose `Reset password`, read the local access-code email from `C:\data\temp\email`, set a password, and sign in. Keep `Consumer` open while you edit content in `Composer` to watch runtime updates land live.
6. Start [src/Samples/Jobs/Engine/Engine.csproj](src/Samples/Jobs/Engine/Engine.csproj) alongside `Catalog` or `Composer` when you want to study jobs. The UI hosts expose `Jobs` and `Schedules` panes, and the engine creates, runs, and publishes job updates back into those panes.

## Why Crudspa

| Area | What It Gives You |
| --- | --- |
| Navigation shell | A rich app shell for browser-friendly, deep-linkable workflows instead of a pile of disconnected pages. |
| View, Model, Services | A practical UI architecture that keeps rendering, UI state, and feature access readable as screens grow. |
| Typed service boundary | Explicit `Request<T>` and `Response<T>` contracts for remote calls, with wrappers for cross-cutting concerns. |
| Real-time flow | SignalR-based notices so connected users see changes quickly and predictably. |
| Data-layer support | A clear database story for transactions, predicates, validation, and durable business rules. |
| Pluggable modules | An opt-in architecture that can power framework code, content systems, jobs, and domain modules without rewriting the core. |

## Repository

This repository is organized into a few major areas, each with a different job:

| Area | Purpose |
| --- | --- |
| [src/Framework](src/Framework) | Core platform libraries for navigation, contracts, models, wrappers, components, sessions, security hooks, and background jobs. |
| [src/Content](src/Content) | Higher-level modules for content authoring, runtime content delivery, and content-specific jobs. |
| [src/Education](src/Education) | Larger domain-shaped modules that show the same architecture applied to real business features. |
| [src/Samples](src/Samples) | Focused sample hosts for `Catalog`, `Composer`, `Consumer`, and the `Jobs Engine`. |
| [src/Database](src/Database) | The shared SQL Database Project, including schema, views, functions, triggers, and stored procedures. |
| [doc](doc) | The primary documentation set for architecture, concepts, patterns, components, styling, types, and database guidance. |

Crudspa is intentionally layered. The reusable libraries are the center of gravity, the sample applications provide focused end-to-end walkthroughs, and the architecture also scales into broader `Portals`, `Sites`, and jobs solutions.

## Samples

The sample track is meant to give new readers a short path from clone to understanding:

| Sample | What It Shows |
| --- | --- |
| `Catalog` | A focused `Framework.Core` application with a public shopping cart, private catalog and order-management surfaces, and sample jobs administration panes. |
| `Composer` | How `Content.Design` adds editor-facing CMS workflows, metadata-driven panes, authoring services, and sample jobs administration panes. |
| `Consumer` | How `Content.Display` layers authored runtime experiences onto the same base architecture and reacts to content changes from `Composer`. |
| `Jobs Engine` | The scheduler and worker host that completes the jobs story by running background work and publishing job events back into the sample UIs. |

When you want the smallest complete story, start with `Catalog`. Then run `Composer` and `Consumer` together for the content story, and add `Samples/Jobs/Engine` when you want to see background work move through the same architecture.

For the host-by-host walkthrough, see [doc/Overview/Samples.md](doc/Overview/Samples.md).

## Start

The fastest way to get oriented is:

1. Read [doc/Starting.md](doc/Starting.md) for the solution map and the sample-driven mental model.
2. Read [doc/Overview/Architecture.md](doc/Overview/Architecture.md) for the end-to-end request, shell, and real-time flow.
3. Read [doc/Overview/Applications.md](doc/Overview/Applications.md) if you want to see how the reusable libraries come together in real applications.
4. Read [doc/Overview/Vocabulary.md](doc/Overview/Vocabulary.md) if you want the core Crudspa terms before diving into the source.
5. Run `Catalog` first, then `Composer` plus `Consumer`, and finally `Samples/Jobs/Engine` when you want the background-work story.

Managed projects in this repository target `net10.0`.

For the full workspace, open [src/Crudspa.slnx](src/Crudspa.slnx) in Visual Studio. The solution includes [src/Database/Database.sqlproj](src/Database/Database.sqlproj), so full solution builds require Visual Studio or MSBuild with SQL Server Data Tools. Plain `dotnet build` is still useful for managed projects, but a full solution build will stop when it reaches the SQL project if SSDT isn't installed.

## Docs

The documentation is meant to teach both the platform's shape and the reasoning behind it. These are the best entry points from the top-level README:

| If You Want To... | Start Here |
| --- | --- |
| Build the repo and understand what is here | [doc/Starting.md](doc/Starting.md) |
| Choose the right sample application | [doc/Starting.md](doc/Starting.md) |
| Run the sample track deliberately | [doc/Overview/Samples.md](doc/Overview/Samples.md) |
| See the platform at a glance | [doc/Overview/Architecture.md](doc/Overview/Architecture.md) |
| See how the libraries fit into real applications | [doc/Overview/Applications.md](doc/Overview/Applications.md) |
| Learn the guiding principles | [doc/Overview/Philosophy.md](doc/Overview/Philosophy.md) |
| See the project's likely direction | [doc/Overview/Roadmap.md](doc/Overview/Roadmap.md) |
| Learn the core terms | [doc/Overview/Vocabulary.md](doc/Overview/Vocabulary.md) |
| Browse the full documentation set | [doc/ReadMe.md](doc/ReadMe.md) |
| Review SQL conventions | [doc/Databases/Standards.md](doc/Databases/Standards.md) |

## Open Source

Crudspa welcomes focused contributions that solve real problems while fitting the platform's existing structure. If you are considering a pull request, start with the contribution guide and keep documentation in sync with behavior changes.

Questions, bug reports, and small improvement proposals belong in GitHub Discussions. Security reports and other private concerns should follow the contact paths in [Security.md](Security.md) and [Support.md](Support.md).

| Topic | Link |
| --- | --- |
| Contributing | [Contributing.md](Contributing.md) |
| Code of Conduct | [CodeOfConduct.md](CodeOfConduct.md) |
| Security | [Security.md](Security.md) |
| Support | [Support.md](Support.md) |
| License | [License.txt](License.txt) |

Crudspa is released under the MIT license. Third-party notices are available under [lic/ThirdParty](lic/ThirdParty).

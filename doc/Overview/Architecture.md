# Overview | Architecture

CRUD+SPA applications ask teams to solve the same platform problems again and again: navigation, screen state, service boundaries, permissions, real-time refresh, and data enforcement. Those concerns are necessary, but they are rarely the business problem the team actually set out to solve.

Crudspa gives .NET developers a strong starting architecture for that class of software. It provides a coherent default shape for Blazor WebAssembly clients, SignalR-based service boundaries, shared C# contracts, and SQL Server-backed data work so teams can focus on domain behavior instead of rebuilding the same foundation for each solution.

The repository teaches that architecture in layers. `Catalog` is the focused `Framework.Core` sample. `Composer` and `Consumer` show what changes when `Content.Design` and `Content.Display` are added. The jobs story is split between jobs panes in `Catalog` or `Composer` and the `Samples/Jobs/Engine` host that schedules and executes work. The larger domain modules show how those ideas hold up in broader solutions.

The architecture is organized along two axes:

* vertical separation by runtime boundary: `Client`, `Shared`, `Server`, and `Database`
* horizontal separation by feature boundary: `Area`, `Module`, and `Node`

Crudspa is a plugin-driven modular monolith organized around orthogonal separation of concerns. It combines layered runtime boundaries, vertical feature slices, a deep-linkable application shell, typed service boundaries, and an explicit data layer. The point of that structure is straightforward: common CRUD+SPA concerns already have a home, so solution teams can spend their time on the parts of the system that are actually specific to their business.

## Why This Shape Works

| Benefit | Why It Happens |
| --- | --- |
| Predictable change surface | New behavior usually lands in one feature slice and one layer boundary instead of leaking across the entire app. |
| Reusable hosts | The same contracts and patterns can support rich portals, lighter sites, and optional jobs or worker hosts. |
| Cleaner cross-cutting concerns | Session handling, permissions, logging, retries, validation, and notice publishing live in wrappers and shared infrastructure instead of every feature. |
| Stronger enforcement | Server services and SQL Server reinforce permissions, predicates, tenancy, and transactions instead of relying on UI-only checks. |
| Searchable and refactor-friendly structure | Stable naming, folder layout, and boundary patterns make it easier for developers, editors, and automation tools to find the right place for new behavior. |

This shape works especially well when a product needs many CRUD features over time, strict permissions or tenancy, real-time screens, and multiple app shapes that should still feel like one platform.

## Modern And Distinctive

Crudspa is also a distinctly modern .NET architecture. It assumes a C# client in the browser, typed contracts shared across the boundary, SignalR for both request and notice flow, and a data layer that stays explicit rather than hidden behind a vague persistence abstraction.

* `Blazor WebAssembly` puts a real C# client in the browser, so views, client models, and shared contracts can stay in one language and one toolchain.
* `SignalR` is not just a notification add-on. Crudspa uses it as the typed service transport to the server and as the notice channel for real-time refresh.
* `Shared` contracts let the browser and server exchange the same typed DTOs, events, config payloads, and error structures.
* The shell stays deep-linkable and browser-friendly while still behaving like a rich multi-pane application.
* JavaScript interop stays close to the edges where browser APIs genuinely require it instead of becoming the center of the client architecture.
* The default stack includes a SQL Database Project and explicit server-side data access, so modern client behavior does not come at the cost of weak data enforcement.
* Most solutions are best implemented as modular monoliths with strong internal boundaries, which keeps them easier to evolve than prematurely distributed systems.

What is distinctive is how those parts stay aligned inside the same feature slice. A `Node` can participate in navigation, typed services, real-time notices, and SQL enforcement without inventing its own local architecture.

## Two Axes

The vertical axis is about runtime responsibility:

* `Client` owns the shell, `Pane` plugins, client models, and thin service proxies.
* `Shared` owns DTOs, behavior contracts, event payloads, and config contracts.
* `Server` owns hubs, wrappers, application services, repositories, and sproxies.
* `Database` owns schema, views, functions, procedures, and durable data rules.

The horizontal axis is about feature structure.

An `Area` is a top-level source family such as `Framework`, `Content`, or `Education`. A `Module` is a cohesive slice inside an `Area` such as `Design`, `Display`, or `District`. A `Node` is Crudspa's bounded feature slice around one root entity and the queries, mutations, contracts, related `Pane` work surfaces, and notices that belong to it.

That two-axis structure looks like this:

| Layer | Area-Level Shape | Module-Level Shape | Node-Level Work |
| --- | --- | --- | --- |
| `Client` | shared shell and client primitives | `src/<Area>/<Module>/Client` | `Pane` plugins, models, local helpers, service proxies |
| `Shared` | cross-layer contracts | `src/<Area>/<Module>/Shared` | DTOs, behavior contracts, events, config |
| `Server` | wrappers and server infrastructure | `src/<Area>/<Module>/Server` | hubs, services, repositories, sproxies, notice publishing |
| `Database` | shared relational foundation | `src/Database/<Area><Module>` | tables, views, functions, procedures, predicates, transactions |

Every meaningful feature has an obvious home in this structure. Layered concerns stay layered, feature concerns stay feature-oriented, and neither axis has to collapse into the other.

On top of that internal structure, users experience the system through the shell vocabulary. A `Portal` defines the top-level application shell. Each `Segment` structures that shell into navigable branches. Each `Pane` is an actual work surface. A `Pane` usually fronts one `Node` or one report, which is how the shell stays deep-linkable without becoming a pile of unrelated routes.

You can read that structure in two directions. From the user's side, a path resolves through `Portal`, `Segment`, and `Pane` until it reaches a `Node` or report. From the implementation side, that same feature crosses `Client`, `Shared`, `Server`, and `Database`. The shell gives users a stable way to reach work, while the runtime layers give developers a stable place to implement it.

## One Slice

If you want the smallest end-to-end slice first, start with `Catalog`. If you want a built-in framework example that already exercises content authoring layers, `Track` is a strong next stop.

To make the structure concrete, the `Track` feature in `Content.Design` spans these files:

| Concern | Example |
| --- | --- |
| Pane and model | [`src/Content/Design/Client/Plugins/PaneType/TrackEdit.razor.cs`](../../src/Content/Design/Client/Plugins/PaneType/TrackEdit.razor.cs) |
| Behavior contract | [`src/Content/Design/Shared/Contracts/Behavior/ITrackService.cs`](../../src/Content/Design/Shared/Contracts/Behavior/ITrackService.cs) |
| Client proxy | [`src/Content/Design/Client/Services/TrackServiceTcp.cs`](../../src/Content/Design/Client/Services/TrackServiceTcp.cs) |
| Server boundary | [`src/Content/Design/Server/Hubs/TrackServiceHub.cs`](../../src/Content/Design/Server/Hubs/TrackServiceHub.cs) |
| Server orchestration | [`src/Content/Design/Server/Services/TrackServiceSql.cs`](../../src/Content/Design/Server/Services/TrackServiceSql.cs) |
| SQL data work | [`src/Database/ContentDesign/StoredProcedures/TrackSelect.sql`](../../src/Database/ContentDesign/StoredProcedures/TrackSelect.sql) |

Notice what stays stable as the request crosses the stack: the feature name, the boundary contract, the permission scope, and the SQL artifacts all line up. That consistency is what lets a `Module` grow large without becoming mysterious.

## Request Loop

A typical Crudspa request flow looks like this:

1. A user opens a path that resolves to a `Portal`, `Segment`, and `Pane`.
2. The `Pane` and its client model load UI state and call a shared behavior contract through a thin proxy.
3. The proxy sends a `Request<T>` envelope over the hub boundary.
4. The hub applies session and permission wrappers at the trust boundary.
5. The server service validates, orchestrates, and calls repositories or sproxies.
6. SQL Server performs the authoritative data work.
7. A `Response<T>` returns to the client.
8. After a successful mutation, the host can publish a typed notice so other clients refresh or reconcile local state, and it can publish a typed gateway event when another host needs to react.

This is not just a request pipeline. It is the platform's consistency loop. Navigation, service calls, SQL enforcement, and real-time refresh all follow the same architecture instead of competing with it.

In practice, one pane action usually moves from model to proxy, through hub wrappers, into a server service, down to repositories or sproxies and SQL, then back as a `Response<T>`. If the mutation succeeds, that same feature can publish a typed notice so other models refresh or reconcile local state. When another host also needs to react, the same feature can publish a typed gateway event so the other host can invalidate caches and rebroadcast its own notice. Requests and notices therefore live inside one architectural loop instead of two unrelated systems.

## Shell And Client

Crudspa's preferred UI shape is View, Model, Services.

Views are Blazor components, usually split into `.razor` markup and `.razor.cs` code-behind. Models hold UI-only state and workflow behavior such as waiting state, alerts, modal visibility, list state, edit state, and event handling. Services provide either local helpers or remote feature access.

The shell itself is data-driven and plugin-based:

* `Portal` metadata selects navigation behavior
* `Segment` metadata selects `Segment` display behavior
* `Pane` metadata selects `Pane` display behavior

That is why the platform can support very different application shapes without rewriting the shell for each one. The `Navigator` service keeps URL state, screen state, and titles aligned so deep links, the back button, and multi-pane workflows still feel like normal browser behavior.

## Service Boundary

Crudspa is opinionated about remote boundaries.

`Shared` defines the behavior contracts. Client proxy services stay extremely small. Hubs stay focused on authorization, session handling, and notice publishing. Server services own the actual orchestration and business behavior. In modern .NET terms, this looks closest to typed application services behind a clean RPC boundary.

The `Request<T>` and `Response<T>` envelopes matter because they make that boundary predictable. Requests carry one typed payload plus session context. Responses carry either a typed result or structured errors. That stable shape is what makes wrappers so effective for logging, retries, exception handling, validation, and other cross-cutting concerns.

## Data Layer

Crudspa keeps the data layer explicit.

The SQL Database Project is part of the default architecture, not a sidecar. Repositories and sproxies keep data access visible. In the standard setup, teams can work on C# and database code side by side in Visual Studio and SSDT, which makes the data layer easier to inspect, refactor, and enforce deliberately.

That does not mean Crudspa is locked to one storage strategy. The service boundary is interface-driven and DI-friendly, so any `IService` can be replaced with an implementation backed by a different store when that fits the problem better. The point of the default setup is clarity and productivity, not insisting that every solution use one database forever.

When teams do follow the default path, predicates, scope checks, tenancy rules, and transactional writes stay close to the data tier where they can actually be enforced.

This matters for correctness. A permission or scope rule that exists only in `Pane` code is not a real rule. Crudspa expects important data behavior to be enforced again on the server and in SQL Server.

## Fresh State

Real-time behavior is one of the platform's strongest architectural qualities.

After a successful mutation, the server can publish a typed notice over SignalR. Subscriptions happen in the context of a real session, so the audience can be scoped by permissions and other session-aware rules. On the client, models decide whether to refresh, replace, remove, or ignore local state.

When the reacting client lives in another host, Crudspa keeps the same event vocabulary and adds one gateway hop. `Composer` publishes `PageContentChanged` and `PortalRunChanged` so `Consumer` can invalidate runtime caches and rebroadcast. `Samples/Jobs/Engine` publishes job and schedule events so `Catalog` and `Composer` can refresh their `Jobs` panes. In the local sample configuration, those gateway publishes go to the checked-in receiver URLs on the sample web hosts. In broader deployments, the same abstraction can target Event Grid.

That keeps lists, edit panes, and runtime content from going stale in multi-user scenarios. Just as important, it keeps real-time behavior inside the same service architecture instead of creating a second, hidden synchronization system.

## Where It Shines

Crudspa is at its best when you are building:

* long-lived line-of-business portals that will accumulate many CRUD features
* products that mix admin tooling, runtime content, and background jobs
* multi-tenant or permission-heavy systems where trust boundaries must stay explicit
* rich, real-time applications where stale screens create operational friction
* codebases that benefit from strong conventions for extension, refactoring, and AI-assisted implementation

If your application is tiny, mostly static, or intentionally short-lived, this is more architecture than you need. Crudspa pays off when changeability matters for years, not days.

## Tradeoffs

Crudspa is more structured than a minimal Blazor application, and that is intentional. Metadata-driven plugin selection means some mistakes appear at runtime instead of compile time. The built-in database project is most comfortable for teams that want application and database code visible in the same development environment. And the platform asks you to learn its vocabulary instead of pretending all CRUD systems are the same.

Crudspa is aimed at application surfaces with workflows, state, identity, and long-lived data. Teams sometimes still choose it for public-facing experiences when the CMS, workflow, and long-term maintenance benefits outweigh the initial download cost.

The payoff is coherence. Navigation, services, real-time behavior, and SQL all reinforce each other, which is exactly what large CRUD+SPA systems usually struggle to preserve.

## Next Steps

* [Overview | Vocabulary](Vocabulary.md)
* [Concepts | Navigation](../Concepts/Navigation.md)
* [Concepts | Services](../Concepts/Services.md)
* [Concepts | Notices](../Concepts/Notices.md)
* [Documentation Index](../ReadMe.md)

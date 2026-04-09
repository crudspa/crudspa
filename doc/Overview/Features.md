# Overview | Features

When people first encounter Crudspa, the natural question is simple: what does this framework actually give me beyond a set of reusable Blazor components?

The answer is that Crudspa is an end-to-end CRUD+SPA platform. It does not stop at widgets, routing, or service proxies. It gives you a coherent shell, a client model layer, a server boundary, a real-time and cross-host update story, a clear data-layer story, and higher-level modules for content and jobs.

At a high level, Crudspa stacks four feature families. The shell and UX layer covers portals, segments, panes, models, and reusable components. The boundary layer covers contracts, proxies, hubs, wrappers, and notices. The data layer covers the SQL project, repositories, sproxies, and predicates. On top of those foundations sit higher-level modules for content, jobs, theming, and broader domain features. Crudspa is cumulative rather than siloed: higher-level modules build on the same shell, boundary, and SQL foundations instead of replacing them.

## Core Feature Areas

| Feature Area | What It Gives You |
| --- | --- |
| Navigation shell | a deep-linkable `portal`, `segment`, and `pane` model that supports rich app navigation without giving up normal browser behavior |
| Service boundary | explicit `Request<T>` and `Response<T>` contracts, thin client proxies, server hubs, wrappers for cross-cutting concerns, and clear seams for observability and resilience |
| Real-time updates | SignalR-backed notices inside a host, plus gateway publishes when another host needs to invalidate caches or rebroadcast typed events |
| Sessions, security, and tenancy | session-aware requests, permission checks, and scope enforcement that belong to the architecture instead of living only in UI code |
| Typed model and component layer | reusable components plus client models for alerts, waiting states, forms, lists, tabs, modals, and navigation workflows |
| Data-layer patterns | a SQL Database Project plus the default stored procedure and repository patterns for validation, scope, and transactions |
| Styling and theme support | a broad styling foundation plus typed content styles, fonts, and brand settings that can flow into runtime hosts |
| Reports, jobs, and content modules | higher-level capabilities for read-heavy views, background operations, content authoring, and content delivery |

## Why These Features Fit Together

Crudspa is at its best when those features are used together instead of cherry-picked in isolation.

The navigation shell gives structure to the UI. The model layer keeps views focused. The service boundary keeps client and server concerns honest. The data layer keeps business rules and data integrity close to the data. Real-time notices tie the whole thing together so the app stays responsive in multi-user scenarios.

That is why the platform feels more like a working architecture than a bag of helpers.

This is also why Crudspa is designed for interactive, stateful, data-heavy surfaces where that full stack pays for itself.

## See It In Practice

The feature list lands faster when you pair it with the right sample:

* `Catalog` is the shortest path to the `Framework.Core` story: shell, models, services, notices, the core data-layer patterns, and the simplest jobs administration surface.
* `Composer` shows editor workflows, metadata-driven panes, and authoring services layered on the same contracts, and it is the authoring half of the live `Composer`-to-`Consumer` loop.
* `Consumer` shows binders, pages, elements, media, and theme-aware runtime delivery, and it is best studied while `Composer` is changing content.
* `Jobs Engine` handles scheduling and operational processing while `Catalog` and `Composer` expose the `Jobs` and `Schedules` panes.
* The larger domain modules show how the same feature families scale into broader application shapes.

## Choosing Where To Read Next

Pick your next stop based on the question you are trying to answer:

* If you want the end-to-end picture, read [Overview | Architecture](Architecture.md).
* If you want the shell, read [Concepts | Navigation](../Concepts/Navigation.md).
* If you want the service boundary, read [Concepts | Services](../Concepts/Services.md), [Concepts | Wrappers](../Concepts/Wrappers.md), and [Concepts | Observability](../Concepts/Observability.md).
* If you want runtime updates, read [Concepts | Notices](../Concepts/Notices.md).
* If you want higher-level module guidance, read [Overview | Libraries](Libraries.md) and [Concepts | Plugins](../Concepts/Plugins.md).
* If you want operations and background work, read [Concepts | Jobs](../Concepts/Jobs.md).

## Next Steps

* [Overview | Architecture](Architecture.md)
* [Overview | Libraries](Libraries.md)
* [Overview | Vocabulary](Vocabulary.md)
* [Documentation Index](../ReadMe.md)

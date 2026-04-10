# Overview | Philosophy

CRUD+SPA systems tend to become difficult when navigation, UI state, service boundaries, and database rules evolve as separate local solutions. Over time that drift makes the codebase feel harder to change than it should.

Crudspa is built around a different goal. We want rich, responsive, real-time applications that remain readable, pluggable, and easy to reshape as requirements change. That goal drives nearly every architectural choice in the platform.

## What We Optimize For

### Pragmatism

Crudspa is opinionated, but it's not ideological. We prefer approaches that have held up in production over approaches that only look elegant in isolation.

That's why the platform is comfortable being end to end. The shell, the service boundary, the SQL layer, and the operational story are all part of the same design. We don't think the best CRUD+SPA architecture comes from optimizing one layer and hoping the others fall into place.

### Simplicity

We value designs that are easy to explain, easy to extend, and easy to trace. That doesn't mean everything is tiny. It means each part should have a clear job.

In Crudspa, that usually looks like:

* views focused on rendering and composition
* models focused on UI state and workflow
* services focused on feature access
* server code focused on trust-boundary and business behavior
* SQL focused on durable rules, transactions, and data integrity

This is the deeper meaning behind the platform's preference for View, Model, Services. It's a way to keep rich applications understandable.

### Changeability

The most important quality of a codebase is its ability to absorb new reality. Products change. Teams change. Requirements change. Good architecture should make those shifts cheaper instead of turning every new feature into a negotiation with the past.

That's why Crudspa emphasizes:

* stable vocabulary
* explicit boundaries
* plugin-driven extension points
* data-driven navigation
* small, composable layers

A system with a right place to put things is easier to change than a system that relies on heroic memory.

## How That Shows Up In The Platform

The philosophy becomes concrete in a few major ways.

### Rich App Shells

Crudspa is designed for real applications, not just loosely connected pages. The `portal`, `segment`, and `pane` model gives the shell deep linking, back-button support, and a multi-surface workflow without sacrificing browser behavior.

That's a philosophical choice as much as a technical one. We think data-heavy applications deserve a first-class shell.

Crudspa is aimed at application-style experiences where a richer shell earns its keep. Static marketing sites and JavaScript-light browsing have different priorities.

### Real-Time By Default

Many business applications quietly accept stale screens as normal. Crudspa doesn't. Server-sent notices and client-side refresh patterns are part of the architecture because multi-user systems feel better when change is visible quickly and reliably.

This is one of the clearest examples of whole-system thinking. The save flow and the update flow belong to the same design.

### Explicit Service Boundaries

Remote boundaries use typed `Request<T>` and `Response<T>` envelopes. Local-only services use ordinary method signatures. That split is deliberate.

We want trust boundaries to be obvious. We want expected failures to come back as structured responses rather than as surprise crashes. And we want cross-cutting concerns to have a clean place to live.

### Data Layer

Crudspa treats the data layer as part of the application architecture instead of a persistence afterthought. In the standard setup, important rules, predicates, and transactions are enforced close to the data, with a real database project and normal tooling instead of hidden magic.

That stance is practical. Service interfaces and dependency injection keep the architecture pluggable, and the default experience is meant to be clear and productive, especially for .NET teams that want to work on application code and database code side by side in one IDE.

### Pluggable Extension

The platform is designed to be opt-in and extensible. Navigation shells, panes, reports, and other surfaces are plugin-driven so teams can adapt the system without rewriting its center.

This is how Crudspa tries to stay both opinionated and flexible. The framework has a point of view, but it doesn't insist that every application look the same.

## Development Experience

We're deeply optimistic about building frontend and backend behavior in open-source .NET. Blazor WebAssembly, when paired with a disciplined architecture, makes it possible to build rich web applications without splitting the mental model across unrelated stacks.

We also care a lot about tooling. A strong editor, solid refactoring support, SQL project tooling, clear documentation, and focused sample applications all contribute directly to software quality. The repository is meant to feel like a real working environment, not just a code drop.

## Tradeoffs

These choices come with tradeoffs.

Crudspa is more structured than a minimal app scaffold. It asks teams to learn its vocabulary. It's most natural for developers who are comfortable with C#, Blazor, SQL Server, and layered architecture. And because the platform takes the whole system seriously, some features ask for more upfront design than a quick one-off implementation would.

We think those trades are worth making. They buy clarity, reuse, and a calmer path for future change.

## Next Steps

* [Overview | Architecture](Architecture.md)
* [Overview | Features](Features.md)
* [Concepts | Navigation](../Concepts/Navigation.md)
* [Concepts | Services](../Concepts/Services.md)
* [Documentation Index](../ReadMe.md)

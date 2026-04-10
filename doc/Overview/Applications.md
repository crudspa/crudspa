# Overview | Applications

Crudspa is easiest to understand as a way to build applications, not as a pile of reusable parts. You don't need to master the whole framework before it becomes useful. The usual path is simpler: choose the modules that fit your problem, follow the platform's default application shape, and keep your own code focused on domain behavior, screen workflow, and product-specific decisions.

That's what the sample applications are for. They aren't the point of Crudspa by themselves. They are compact examples of how to assemble the libraries into clean, maintainable applications.

## Start With The Right Application

The sample track is meant to shorten the path from clone to understanding:

* `Catalog` is the best starting point when you want a focused `Framework.Core` application and the simplest sample to run first.
* `Composer` is the best starting point when you want editor-facing content management, especially with `Consumer` open beside it.
* `Consumer` is the runtime half of the content story, so it's best studied together with `Composer`.
* `Samples/Jobs/Engine` is the background-work host. Pair it with `Catalog` or `Composer`, both of which expose `Jobs` and `Schedules` panes.

If you are building a conventional admin-style CRUD+SPA, start with `Catalog`. It shows the cleanest path to the core shell, model, service, and data-layer story without asking you to absorb the content stack at the same time.

The jobs story is intentionally split. The UI for creating and inspecting jobs lives in the same web hosts where users already work. The dedicated engine host under `src/Samples/Jobs` handles scheduling and execution. That keeps background work inside the same architecture without forcing a separate jobs website.

For the concrete host-by-host walkthrough, read [Overview | Samples](Samples.md).

## What Your Application Usually Adds

Crudspa is intentionally modular, so your application code can stay focused on the parts that are truly yours.

In practice, an application usually adds:

* branding, navigation metadata, and overall product shape
* pane and report plugins for your own workflows
* domain-specific services, DTOs, and database objects
* composition choices around authentication, infrastructure providers, and optional modules

That's the public story we want the repo to tell: the framework handles the recurring platform problems cleanly, while your application stays free to concentrate on the business itself.

## What The Libraries Already Give You

The reusable libraries already cover most of the plumbing that usually makes CRUD+SPA applications messy:

* a deep-linkable navigation shell built around portals, segments, and panes
* a practical client shape based on View, Model, Services
* a typed remote boundary with predictable request and response handling
* real-time notices so multi-user screens stay current
* a clear relational data story for validation, tenancy, transactions, and durable data rules

That means application code can start at a much higher level than "wire up routing, modals, alerts, and hub calls from scratch."

## Choosing Modules

Crudspa is designed to be adopted in layers.

* Start with `Framework.Core` when you need the shell, components, typed services, sessions, and a solid data-heavy CRUD foundation.
* Add `Framework.Jobs` when you need background scheduling and worker processes.
* Add `Content.Design` when you need authoring, metadata-driven panes, and editor-facing content workflows.
* Add `Content.Display` when you need runtime content delivery, binders, elements, and theming.

That progression is one of the framework's main strengths. Teams can start small without painting themselves into a corner.

## Where To Put Your Code

A healthy Crudspa application usually keeps a clear split between reusable framework code and app-specific code.

As a rule of thumb:

* keep framework and shared module behavior in the library layers
* keep application-specific composition and branding in the application projects
* keep your own panes, services, and database changes aligned with the same layer and feature conventions the repo already uses

That's one of the reasons the framework stays readable at scale. New work usually has an obvious home.

## Reading Order

If you are evaluating Crudspa for your own application, this sequence works well:

1. Read [Getting Started](../Starting.md).
2. Read [Overview | Architecture](Architecture.md).
3. Read [Overview | Libraries](Libraries.md).
4. Run the sample or sample pair that matches your immediate goal.
5. Follow one feature from pane, to model, to service, to SQL.

That path teaches the framework as a practical tool, not as a pile of abstractions.

## Next Steps

* [Getting Started](../Starting.md)
* [Overview | Samples](Samples.md)
* [Overview | Architecture](Architecture.md)
* [Overview | Libraries](Libraries.md)
* [Concepts | Injection](../Concepts/Injection.md)
* [Documentation Index](../ReadMe.md)

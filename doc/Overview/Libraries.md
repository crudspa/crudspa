# Overview | Libraries

One of the easiest ways to get lost in Crudspa is to treat every folder as if it had the same job. It does not. Some projects are core platform libraries. Some are higher-level reusable modules. Some are focused sample applications. And the domain modules show how those pieces can come together in broader product-style compositions.

Once you read the repo in those families, the structure becomes much easier to navigate.

## Library Families

| Family | What It Gives You | Best Starting Point |
| --- | --- | --- |
| `Framework.Core` | shell, navigation, contracts, wrappers, components, client models, sessions, security hooks, and the main client or server boundary | [src/Framework/Core](../../src/Framework/Core) |
| `Framework.Jobs` | job records, schedules, worker and scheduler infrastructure, client admin surfaces, and job contracts | [src/Framework/Jobs](../../src/Framework/Jobs) |
| `Content.Design` | editor-facing content administration for portals, tracks, courses, pages, sections, styles, fonts, and related assets | [src/Content/Design](../../src/Content/Design) |
| `Content.Display` | runtime delivery of authored content, including binders, pages, sections, elements, progress, and media-aware display behavior | [src/Content/Display](../../src/Content/Display) |
| `Content.Jobs` | content-specific background work layered on top of the jobs framework, such as operational email workflows | [src/Content/Jobs](../../src/Content/Jobs) |
| `Education.Common` | shared domain helpers and contracts that support multiple education-focused modules | [src/Education/Common](../../src/Education/Common) |
| `Education.*` modules | realistic domain modules such as district, provider, publisher, school, and student, each built with the same client, shared, and server split | [src/Education](../../src/Education) |

## Samples And Hosts

Crudspa is easier to understand when you separate reusable libraries from the applications that compose them.

| Shape | What It Shows |
| --- | --- |
| `Catalog` | a focused `Framework.Core` application with a public shopping cart, private catalog and order-management surfaces, and sample jobs administration panes |
| `Composer` | how `Content.Design` adds editor-facing authoring workflows, pane metadata, content administration, and sample jobs administration panes |
| `Consumer` | how `Content.Display` adds runtime content, pages, elements, media, and theming on the same foundation |
| `Jobs Engine` | how the jobs stack schedules and executes operational work while publishing updates back into the web hosts |
| `Education` modules | how the same libraries scale into broader compositions with richer feature sets |

## How The Families Relate

`Framework.Core` is the center of gravity. Nearly everything else depends on its shell, contracts, services, wrappers, and UI patterns.

`Framework.Jobs` extends that same architecture into background work. It does not introduce a different mental model.

In the shipped samples, that jobs story spans multiple hosts on purpose. `Catalog` and `Composer` provide the admin panes where users create and inspect jobs or schedules. `src/Samples/Jobs/Engine` is the focused scheduler and worker host that makes those records move.

`Content.Design` and `Content.Display` sit one layer higher. They reuse the framework fundamentals and apply them to authoring and runtime content. `Content.Jobs` then applies the jobs infrastructure to content-specific operational needs.

The education modules show what this looks like in a domain. They are not a separate platform. They are proof that the same approach holds up in real business features.

## Reusable Versus App-Specific Code

The projects in this repository are mostly reusable libraries. They define patterns, contracts, components, services, and modules that can be used in different applications. The sample applications then provide the final composition that makes those layers tangible.

A sample or full application host typically adds:

* a root `App.razor`
* a `Program.cs` that boots the website or worker
* a `Registry.cs` that registers the chosen modules
* custom branding, navigation metadata, and project-specific pane plugins

In larger Crudspa solutions, those host layers often live under `Portals` or `Sites`. That is where a specific product, brand, or audience begins to show. The libraries stay reusable underneath.

If you want to see that application story directly, read [Overview | Applications](Applications.md).
If you want the concrete host walkthrough, read [Overview | Samples](Samples.md).

## Practical Reading Order

If you are new, this order works well:

1. Read `Framework.Core` first.
2. Open `Catalog` to see how that foundation becomes a working application.
3. Read `Framework.Jobs` only if background work matters to your current task.
4. Choose `Composer`, `Consumer`, or run `Samples/Jobs/Engine` alongside `Catalog` or `Composer` depending on whether you care more about authoring, runtime delivery, or background work.
5. Read `Education.Common` and one domain module to see the patterns in a realistic slice.
6. Finally, move into the larger domain modules if you want to see how the libraries compose into broader feature sets.

That order preserves the framework's layering instead of flattening it into one big tree.

## Next Steps

* [Overview | Features](Features.md)
* [Overview | Applications](Applications.md)
* [Overview | Samples](Samples.md)
* [Overview | Architecture](Architecture.md)
* [Getting Started](../Starting.md)
* [Documentation Index](../ReadMe.md)

# Overview | Roadmap

Open-source .NET teams usually want more than a feature list before they invest time in a framework. They want to know what the maintainers are trying to build, which parts look stable, and whether the next wave of work deepens the current architecture or pulls it in a different direction.

This page answers that question in the same shape as the repository itself: `Framework`, `Content`, and `Education`. The roadmap is directional rather than calendar-driven. Crudspa is maintained by a small team, so priorities can move when production needs, documentation gaps, or strong community contributions change the most useful next step.

> [!NOTE]
> This page is not a release schedule. It describes the most likely direction of the platform based on the current codebase, documentation, and long-term intent.

## Framework

The long-term goal for `Framework` is straightforward: grow it into a premier open-source .NET foundation for serious CRUD+SPA systems. The ambition is broad, but the implementation style should stay pragmatic. Developers should be able to land in the codebase, trace a feature end to end, and become productive quickly. Hosts should be able to swap implementations at the edges without framework surgery. Common work should feel clearer and faster, not more ceremonial.

The most likely next steps in this area are:

* Widen provider seams at the application boundary so services such as email, texting, caching, blob storage, and similar operational concerns remain easy to replace.
* Preserve the framework's bias toward readable feature slices, obvious extension points, and low cognitive overhead as the codebase grows.
* Improve day-to-day Linux- and macOS-based development support so more teams can work comfortably with the managed projects, docs, and surrounding toolchain from non-Windows environments.
* Explore additive expansion paths, including alternate database implementations such as PostgreSQL, without disrupting the current database story for existing users.
* Explore SQLite-backed application shapes for fully contained deployments where shipping the app and its data together is the right fit.
* Explore packaging paths for mobile and desktop applications, likely starting with MAUI and possibly WPF, so the same overall application model can travel beyond the browser.
* Expand authentication options with SSO and other sign-in flows beyond the current defaults.
* Broaden the built-in provider story around Redis-style caching plus alternate email and texting providers.

## Content

The `Content` roadmap is to grow the current design and display modules into a complete open-source CMS, not just a set of content helpers. Crudspa already has the core vocabulary and many of the moving parts: binders, pages, sections, elements, rules, blogs, forums, threads, posts, authoring panes, and runtime delivery. The work ahead is mostly about making those pieces feel more complete, more coherent, and easier to understand as one system.

The most likely next steps in this area are:

* Simplify authoring by consolidating page-editing flows that already share base models and component patterns.
* Strengthen the public story around authored runtime delivery so binder, page, section, element, and rule composition is easier to follow.
* Make community and publishing features such as blogs, forums, threads, and posts feel more first-class in the docs and sample track.
* Keep `Content.Design` and `Content.Display` aligned so authored experiences and runtime rendering continue to read as one architecture instead of two separate stacks.

## Education

The `Education` roadmap is to grow the existing learning modules into a complete open-source LMS built on the same shared foundation. The repo already contains real learning primitives rather than placeholders: tracks, courses, progress, completions, achievements, notebooks, assignments, classrooms, and reports. The next step is to turn that existing vocabulary into a clearer and more complete learning model for publishers, educators, and learners.

The most likely next steps in this area are:

* Deepen authoring and publishing workflows for learning content so the path from course structure to learner experience feels more complete.
* Strengthen learner-facing run flows around progress, completion, notebooks, achievements, and assignment activity.
* Expand educator and administrator surfaces around classrooms, reports, and operational visibility.
* Treat interoperability with outside LMS platforms as a later follow-on, after Crudspa's own learning workflows are even more explicit.

## Contributing

The highest-leverage community contributions usually line up with one of these directions: clearer framework examples, focused provider seams, stronger authentication and application-packaging options, broader cache or communication providers, stronger CMS workflows, tighter runtime content composition, or learning features that make the LMS story more complete without fragmenting the underlying architecture.

Larger cross-stack efforts should still start with a GitHub Discussion first, especially when they touch contracts, client models, server services, SQL artifacts, and documentation at the same time.

## Next Steps

* [Overview | Architecture](Architecture.md)
* [Overview | Features](Features.md)
* [Overview | Libraries](Libraries.md)
* [Contributing](../../Contributing.md)
* [Documentation Index](../ReadMe.md)

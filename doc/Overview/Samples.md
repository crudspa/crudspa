# Overview | Samples

The sample track is the fastest way to understand Crudspa as a working system instead of as a list of libraries.

These aren't throwaway demos. They are focused hosts that show how the platform is actually composed: a small `Framework.Core` app, an authoring host, a runtime host, and a jobs engine. Together they cover most of the architecture that new readers need to see first.

If you haven't already done the one-time setup steps, start with [Getting Started](../Starting.md).

## Sample Map

| Host | Default URL | Sign-In Shape | Best For | Pair With |
| --- | --- | --- | --- | --- |
| `Catalog` | `https://localhost:42100` | required, name-only sample sign-in | the shortest `Framework.Core` walkthrough | `Samples/Jobs/Engine` when you want to study jobs |
| `Composer` | `https://localhost:42200` | optional sign-in, email plus access code and password | editor-facing content management and richer admin composition | `Consumer` and optionally `Samples/Jobs/Engine` |
| `Consumer` | `https://localhost:42300` | sign-in allowed but not required | runtime content delivery and live preview behavior | `Composer` |
| `Samples/Jobs/Engine` | n/a | background host | scheduling and worker execution | `Catalog` or `Composer` |

## Catalog

`Catalog` is the first stop after cloning the repo.

It demonstrates a focused `Framework.Core` application with:

* a public shopping cart flow
* private admin surfaces for books, movies, and shirts
* jobs administration panes
* portal and settings panes that show the shell and metadata story

The sign-in flow is intentionally minimal. Enter any name and the sample creates a temporary session-backed user so you can move straight into the shell, panes, services, notices, and SQL patterns without dealing with passwords or email first.

If you want one end-to-end code tour, start here and trace a book, movie, or shirt feature from pane, to model, to hub, to service, to SQL.

## Composer And Consumer

`Composer` and `Consumer` are meant to be studied together.

`Composer` is the authoring host. It includes seeded surfaces for:

* portal editing
* segments and pane content
* tracks, courses, pages, and sections
* blogs, posts, and post sections
* styles and fonts
* memberships, members, emails, and templates
* jobs and schedules

Those authoring surfaces now support a faster editing loop than they used to. You can duplicate a section from its card menu, and you can copy and paste settings across page, section, element, media item, and button editors when you want to reuse a layout or visual treatment instead of rebuilding it by hand.

`Consumer` is the runtime host. It shows the content-side result of those authoring changes through a smaller runtime portal.

The most useful sample loop is:

1. Open `Composer`.
2. Sign in with `sample@example.com`.
3. Use `Reset password`, read the access-code message from `C:\data\temp\email`, set a password, and finish signing in.
4. Open `Consumer` at the same time.
5. Edit content, duplicate sections, reuse settings, styles, or portal settings in `Composer`.
6. Watch the changes show up in `Consumer`.

This is the shortest path to understanding `Content.Design`, `Content.Display`, portal invalidation, themed runtime output, and the cross-host gateway flow.

## Jobs Engine

The web hosts expose the jobs administration UI. The dedicated engine host runs the scheduling and worker loops.

That split is intentional:

* `Catalog` and `Composer` let you create, inspect, and manage jobs or schedules inside the same UI where the rest of the work already happens.
* `Samples/Jobs/Engine` creates due jobs, runs job actions, updates statuses, and publishes job and schedule events plus live status changes back into the web hosts.

The checked-in sample engine config is designed for local study. The scheduler and worker create anonymous framework sessions on startup, so you can run the engine without supplying a dedicated jobs user in configuration.

If you create or schedule jobs in the UI and nothing advances, the first thing to check is whether `Samples/Jobs/Engine` is running.

## How The Hosts Connect

The shipped sample configuration uses the checked-in `EventReceiverUrls` values in the sample `appsettings.json` files.

That means:

* `Composer` can publish content and portal events that `Consumer` receives and rebroadcasts.
* `Samples/Jobs/Engine` can publish job and schedule events plus live status changes that `Catalog` and `Composer` receive and rebroadcast.

If you keep the default ports `42100`, `42200`, and `42300`, the local gateway flow works with no extra editing.

## Reading Path

Once you have the relevant sample running:

* read [Architecture](Architecture.md) for the full request, notice, and SQL loop
* read [Applications](Applications.md) for the application-composition view
* read [Libraries](Libraries.md) for the repo family map
* read [Concepts | Notices](../Concepts/Notices.md) for cross-host and real-time refresh
* read [Concepts | Jobs](../Concepts/Jobs.md) when the engine host is in play

## Next Steps

* [Getting Started](../Starting.md)
* [Overview | Applications](Applications.md)
* [Overview | Architecture](Architecture.md)
* [Concepts | Jobs](../Concepts/Jobs.md)
* [Documentation Index](../ReadMe.md)

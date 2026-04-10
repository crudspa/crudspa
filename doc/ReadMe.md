# Crudspa Documentation

Crudspa is an end-to-end CRUD+SPA platform, and these docs are written to help teams build with it confidently. Start with the mental model, move through the core architecture, and then drill into the UI and SQL patterns that make real applications hold together.

This page is the best place to orient yourself quickly, then jump into the right level of detail for the work in front of you. The repo pairs reusable libraries with focused sample applications, so the docs should help you choose both the right concept page and the right application to study.

The docs assume an application-first audience. They are written for teams building rich, data-driven software in .NET.

> [!NOTE]
> Looking for the public project overview, positioning, or contact path before you dive into source and samples? Start at [crudspa.org](https://crudspa.org).

> [!TIP]
> New to the repo? Start with [Getting Started](Starting.md), then read [Architecture](Overview/Architecture.md), [Applications](Overview/Applications.md), and [Vocabulary](Overview/Vocabulary.md). That sequence gives you the build story, the system shape, the application model, and the core Crudspa terms before you dive deeper.

## Start Here

| If You Want To... | Read This First | Why It Helps |
| --- | --- | --- |
| Build the repo and understand what is here | [Getting Started](Starting.md) | Explains the layered shape of the repository, the solution map, the sample track, and the fastest path to a clean first build. |
| Pick the right sample app | [Getting Started](Starting.md) | Helps you choose between `Catalog`, `Composer`, `Consumer`, `Jobs Engine`, and the larger domain modules in this repo. |
| Run the sample hosts in the right combinations | [Samples](Overview/Samples.md) | Gives the concrete host-by-host walkthrough for `Catalog`, `Composer`, `Consumer`, and `Samples/Jobs/Engine`. |
| Understand the platform at a glance | [Architecture](Overview/Architecture.md) | Shows how the shell, models, services, real-time flow, and SQL layer fit together. |
| See how Crudspa fits into real applications | [Applications](Overview/Applications.md) | Explains how the reusable libraries turn into clean application shapes, and how the sample applications relate to that story. |
| Learn what Crudspa is optimizing for | [Philosophy](Overview/Philosophy.md) | Explains the practical design choices behind the framework's structure and defaults. |
| See where the project is likely heading | [Roadmap](Overview/Roadmap.md) | Explains which capabilities are likely to deepen next and which architectural bets are expected to stay stable. |
| Learn the platform's core terms | [Vocabulary](Overview/Vocabulary.md) | Defines the words that appear throughout the docs and source, such as `portal`, `segment`, `pane`, and `node`. |
| Browse the platform by capability | [Features](Overview/Features.md) | Summarizes the main feature families from navigation through jobs and content. |
| Navigate the source tree with confidence | [Libraries](Overview/Libraries.md) | Maps the major project families so the repository feels coherent instead of flat. |

## Sample Path

Crudspa is easiest to learn when you match the sample to the question in front of you.

| Sample | Start Here When You Want To... |
| --- | --- |
| `Catalog` | See the smallest complete `Framework.Core` application, including a public-facing flow, private admin panes, and the simplest jobs administration surface. |
| `Composer` | Study CMS authoring, metadata-driven pane composition, content administration workflows, and the authoring half of the live content preview loop. |
| `Consumer` | Study authored runtime pages, binders, elements, media, and theme-aware delivery while `Composer` is changing content. |
| `Jobs Engine` | Study scheduling and operational background-work patterns by running it beside `Catalog` or `Composer`. |
| `Education` modules | See how the same libraries and patterns scale into larger domain-shaped feature sets. |

## Overview

The overview pages give you the big picture first. Read this section when you want the system shape, the repository shape, the platform's design stance, or the shared vocabulary that makes the rest of the docs easier to follow.

| Topic | What It Covers |
| --- | --- |
| [Samples](Overview/Samples.md) | The concrete walkthrough for the sample hosts, including which ones to run together and what each host is meant to teach. |
| [Architecture](Overview/Architecture.md) | The end-to-end shape of a Crudspa application, from the shell and client model layer through hubs, services, and the data layer. |
| [Features](Overview/Features.md) | The major feature families Crudspa provides beyond reusable components, including navigation, real-time updates, wrappers, jobs, and content. |
| [Applications](Overview/Applications.md) | How the reusable libraries fit together in real applications, and how to think about your own application code on top of them. |
| [Libraries](Overview/Libraries.md) | How `Framework`, `Content`, `Education`, and the database project relate, and where each family fits in the stack. |
| [Philosophy](Overview/Philosophy.md) | The production-minded principles behind Crudspa, including pragmatism, simplicity, changeability, and strong data-layer defaults. |
| [Roadmap](Overview/Roadmap.md) | The likely next directions for Crudspa, including integrations, content growth, learning features, and architectural bets expected to remain stable. |
| [Tools](Overview/Tools.md) | The expected .NET, SQL Server, Sass, and editor toolchain for working comfortably in Crudspa. |
| [Vocabulary](Overview/Vocabulary.md) | The canonical terms used throughout the docs and codebase, including shell, feature, and content structure terms. |

## Concepts

The concepts pages explain the platform's core mechanics. This section is where you go when you understand the broad architecture but want to know how Crudspa approaches navigation, services, sessions, real-time behavior, tenancy, or other system-wide concerns.

### Shell And Composition

| Topic | What It Covers |
| --- | --- |
| [Navigation](Concepts/Navigation.md) | The URL-aware shell model for `portal`, `segment`, and `pane` navigation, including deep links and multi-pane workflows. |
| [Models](Concepts/Models.md) | UI-only state machines that keep views focused on rendering while models own waits, alerts, filters, and editing workflow. |
| [Plugins](Concepts/Plugins.md) | The extension model for panes, reports, display surfaces, and shell behavior so applications stay pluggable. |

### Boundaries And Services

| Topic | What It Covers |
| --- | --- |
| [Contracts](Concepts/Contracts.md) | Shared DTOs, behavior interfaces, events, and request or response envelopes that keep client and server aligned. |
| [Services](Concepts/Services.md) | The stateless service patterns Crudspa uses for feature access across client and server boundaries. |
| [Wrappers](Concepts/Wrappers.md) | Cross-cutting hooks for logging, authorization, transactions, observability, and other policies that shouldn't leak into feature code. |
| [Exceptions](Concepts/Exceptions.md) | Why expected failures should travel back as structured results instead of being treated as ordinary control flow exceptions. |
| [Observability](Concepts/Observability.md) | How Crudspa keeps browser logs, server logs, and wrapper-level telemetry inside one standard .NET logging story. |
| [Injection](Concepts/Injection.md) | The dependency registration patterns that keep transports, providers, and project-specific behavior swappable. |

### Runtime And Data Behavior

| Topic | What It Covers |
| --- | --- |
| [Sessions](Concepts/Sessions.md) | Crudspa's session model and how it differs from ad-hoc per-request or per-server state storage. |
| [Security](Concepts/Security.md) | How authentication, permissions, shell filtering, file access, SignalR groups, and SQL enforcement work together. |
| [Tenancy](Concepts/Tenancy.md) | The patterns Crudspa uses to enforce scope when ownership rules are simple, layered, or relationship-driven. |
| [Notices](Concepts/Notices.md) | The SignalR notice patterns that keep multi-user lists, edit panes, and runtime content fresh. |
| [Repositories](Concepts/Repositories.md) | How real saves coordinate root entities, siblings, children, and SQL work without collapsing into one giant service method. |
| [Jobs](Concepts/Jobs.md) | The background scheduling and worker model, kept inside the same architecture rather than pushed into a side system. |

## Patterns

The pattern pages focus on common CRUD work surfaces. They are especially helpful when you are building or reviewing a node and want the standard Crudspa shape for a specific kind of screen.

| Topic | What It Covers |
| --- | --- |
| [Edit](Patterns/Edit.md) | The default record maintenance pattern for loading, validating, saving, and removing one entity and its related data. |
| [Fill](Patterns/Fill.md) | The submission-first pattern for quick repeated entry where save and reset are the main flow. |
| [Find](Patterns/Find.md) | The search-first pattern for typed filters, server-side query execution, paging, sorting, and result cards. |
| [List](Patterns/List.md) | The browse-first collection pattern for reading, navigation, and lightweight collection actions. |
| [Many](Patterns/Many.md) | The repeated-row edit pattern for maintaining multiple related records from one pane with explicit row lifecycle behavior. |

## Components

The component docs are the practical UI reference for everyday Crudspa work. They are organized around common workflows so you can start from the screen you are building and then drill into the exact control family you need.

### Foundations

| Topic | What It Covers |
| --- | --- |
| [Overview](Components/ReadMe.md) | How the component reference is organized and where to start when you are building a new UI surface. |
| [Authentication](Components/Authentication.md) | Sign in, sign out, session prompts, and authentication-related shell UI patterns. |
| [Layouts](Components/Layouts.md) | Reusable structural primitives that keep shells, panes, toolbars, and form layouts visually consistent. |
| [Menus](Components/Menus.md) | Contextual action menus and related command surfaces with consistent trigger and close behavior. |
| [Tabs](Components/Tabs.md) | Tabbed navigation patterns, including URL-aware and nested tab scenarios in pane-heavy interfaces. |

### Forms And Interaction

| Topic | What It Covers |
| --- | --- |
| [Buttons](Components/Buttons.md) | Button intent, labeling, icon placement, destructive actions, and command affordances. |
| [Dialogs](Components/Dialogs.md) | Overlay patterns for edit forms, confirmations, viewers, and error-recovery flows. |
| [Feedback](Components/Feedback.md) | Loading, empty, validation, success, and error states that tell users what the system is doing. |
| [Forms](Components/Forms.md) | Shared form composition patterns for edit or view state, field sizing, nested content, and async saves. |
| [Dropdowns](Components/Dropdowns.md) | Lookup selection patterns with consistent null handling, filtering, and display conventions. |
| [Pickers](Components/Pickers.md) | Structured inputs such as dates, date ranges, colors, and file-name style values. |
| [Selections](Components/Selections.md) | Radio, checkbox, and related choice controls that drive many business rules. |
| [Status](Components/Status.md) | Status pills and status-button editing patterns for workflow-heavy fields. |
| [Textboxes](Components/Textboxes.md) | Text input patterns for debounce, masking, numeric formatting, and rich text safety. |

### Data And Media

| Topic | What It Covers |
| --- | --- |
| [Lists](Components/Lists.md) | List, card, reorder, and many-edit building blocks for high-productivity CRUD screens. |
| [Trees](Components/Trees.md) | Hierarchical selection and expand or collapse behavior for navigation and move flows. |
| [Domain](Components/Domain.md) | Reusable domain-shaped UI for contact, organization, postal, and related business data. |
| [Media](Components/Media.md) | Upload, preview, playback, file fetching, and viewer patterns for media workflows. |

## Styling

The styling pages explain how Crudspa keeps large UI surfaces consistent without forcing every screen into one-off CSS decisions. Use this section when you need to understand layout conventions, bundle composition, or runtime theming.

| Topic | What It Covers |
| --- | --- |
| [Layouts](Styling/Layouts.md) | The responsive layout system that keeps CRUD screens readable across phones, tablets, and desktops. |
| [Stylesheets](Styling/Stylesheets.md) | How module stylesheet entrypoints, host stylesheets, and the shared theme contract work together in real hosts. |
| [Theming](Styling/Theming.md) | Runtime theming with structured design tokens, uploaded fonts, and self-contained previews. |

## Types

The type pages focus on recurring data-shape problems. They are useful when a field seems simple on the surface but carries important UX, validation, storage, or security implications.

| Topic | What It Covers |
| --- | --- |
| [Boolean](Types/Boolean.md) | Defaults, null semantics, and control choices for yes or no fields. |
| [Date/Time](Types/Datetime.md) | Calendar dates, timestamps, ranges, and timezone handling. |
| [Enum](Types/Enum.md) | When code-owned states are a good fit and when lookup data is the better long-term choice. |
| [File](Types/File.md) | Upload, metadata, caching, authorization, and file lifecycle concerns. |
| [Number](Types/Number.md) | Formatting, validation, limits, cadence, thresholds, and ordering behavior. |
| [Relationship](Types/Relationship.md) | Foreign keys, related-entity editing, and relationship-aware save semantics. |
| [Text](Types/Text.md) | Short text, long text, search text, and rich text handling. |

## Databases

Crudspa keeps the database layer visible instead of treating it as a persistence afterthought. These pages explain the database conventions and operational patterns that keep serious CRUD systems correct, observable, and changeable over time.

| Topic | What It Covers |
| --- | --- |
| [Access](Databases/Access.md) | Database access patterns, concurrency expectations, and why the database is a shared runtime dependency. |
| [Auditing](Databases/Auditing.md) | How to answer who changed what and when without bolting on support-only fixes later. |
| [Deletion](Databases/Deletion.md) | Safe deletion strategies and how soft-delete-style behavior fits business systems. |
| [Lookups](Databases/Lookups.md) | Stable small datasets that power dropdowns, radio lists, and filters across the application. |
| [Migrations](Databases/Migrations.md) | The discipline and workflow around safe schema evolution. |
| [Standards](Databases/Standards.md) | Crudspa's SQL naming, structure, and style conventions. |
| [Versioning](Databases/Versioning.md) | Historical row-state patterns for cases where overwrite-only updates aren't enough. |

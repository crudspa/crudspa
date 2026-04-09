# Support

If you are blocked while building with Crudspa, start here. This page explains how to get help efficiently and what kind of support this repository can realistically provide.

Crudspa is maintained by a small team. Open-source support in this repository is best effort and does not include an SLA.

## Start Here

Before starting a GitHub Discussion, read the material that is most likely to answer common setup and architecture questions:

* [crudspa.org](https://crudspa.org) for the public project overview and general contact path
* [doc/ReadMe.md](doc/ReadMe.md)
* [doc/Starting.md](doc/Starting.md)
* [doc/Overview/Samples.md](doc/Overview/Samples.md) for the concrete sample-host walkthrough
* [doc/Databases/Standards.md](doc/Databases/Standards.md) for SQL style and database conventions

When you ask for help in Discussions or by email, include:

* what you expected to happen
* what happened instead
* exact reproduction steps
* relevant logs, stack traces, screenshots, and environment details
* the project or folder you are working in

## Common Sample Checks

Before starting a Discussion about the shipped samples, confirm these basics:

* you published `src/Database/Database.sqlproj` with `src/Database/Deploy-LocalMachine.publish.xml`
* you ran `.\art\SeedSampleBlobs.ps1`
* the sample web hosts are still using `https://localhost:42100`, `https://localhost:42200`, and `https://localhost:42300`, or you updated the matching `EventReceiverUrls` settings everywhere
* if you are signing in to `Composer`, you checked `C:\data\temp\email` for the access-code message
* if you are testing jobs, `src/Samples/Jobs/Engine` is running beside `Catalog` or `Composer`

Those checks resolve a large share of first-run issues quickly.

## Use GitHub Discussions For

GitHub Discussions are the right public place for:

* bugs and regressions
* documentation gaps or inaccuracies
* build and setup problems rooted in this repository
* small, concrete improvement proposals

## Email Instead

Use `support@crudspa.org` for:

* security reports, following [Security.md](Security.md)
* private or data-sensitive support questions
* paid support, custom development, or hosting inquiries

## What We Prioritize

We focus our time where it protects users and keeps real projects moving:

* security and privacy issues
* defects that break existing behavior
* small, reviewable fixes
* focused documentation improvements that unblock real usage

Large redesigns and broad strategic debates are much less likely to get active maintainer time. If your team needs a different direction, a fork may be the right path.

## Response Expectations

We do read everything, but we cannot promise fast turnaround.

Discussions with a clear reproduction, a narrow scope, and a practical path to validation are the easiest for us to address.

## Release Model

Crudspa is source-first today. Most teams work from repository source rather than from a semantically versioned NuGet package line.

That affects support expectations: fixes usually land in current source, not in backported release branches.

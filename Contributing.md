# Contributing

Thank you for your interest in contributing to Crudspa.

Crudspa is a source-first framework maintained by a small team of experienced developers. The best contributions are the ones that solve a real problem while keeping the long-term maintenance cost clear and manageable.

## Before You Start

If you are planning anything larger than a focused bug fix or documentation improvement, start a GitHub Discussion first.

That early conversation matters in this repo. It helps us confirm that an idea fits the framework, matches the current architecture, and is realistic for a small maintainer team to support over time before anyone invests in a larger pull request.

## Good Fits

The contributions most likely to help this repository are:

* bug fixes and regressions
* small, focused framework improvements
* documentation clarifications, corrections, and examples
* additive features that follow the existing structure and naming patterns

## Usually Out of Scope

Some ideas are valid engineering directions, but they are usually not a fit for the main Crudspa repository:

* broad architectural rewrites
* large refactors with little practical user value
* features with a long-term support burden that does not match current team capacity

If your team needs a different direction, a fork is often the right tool.

## Development Setup

Use the real repository shape when developing a change:

* open `src/Crudspa.slnx` in Visual Studio for the full workspace
* expect managed projects to target `net10.0`
* use Visual Studio or MSBuild with SSDT when working on `src/Database/Database.sqlproj`
* use [doc/Starting.md](doc/Starting.md) and [doc/Overview/Samples.md](doc/Overview/Samples.md) for the current sample setup and host relationships
* remember that Crudspa is consumed primarily as source today, not as a semantically versioned NuGet package set

## What We Review For

We review contributions with a practical bias:

* the change solves a real problem
* the solution matches existing Crudspa patterns and vocabulary
* shared contracts, client code, server code, and database code stay coherent when a feature crosses layers
* behavior changes come with documentation updates
* impacted projects build cleanly and the validation steps are explained clearly

## Pull Requests

Please keep pull requests small and easy to review:

* one focused change per pull request
* explain the problem first, then the solution
* include screenshots only when they materially clarify UI behavior
* list the exact build steps and manual validation you performed
* when behavior is sample-facing, validate the smallest relevant host or host pair such as `Catalog`, `Composer` plus `Consumer`, or `Catalog` or `Composer` plus `Samples/Jobs/Engine`
* keep documentation in sync with behavior, naming, and usage changes

For SQL work, follow [doc/Databases/Standards.md](doc/Databases/Standards.md).

## Validation

This repository does not currently rely on a large permanent automated test suite. Most changes are validated through targeted builds, manual verification, and production-minded review.

If you create temporary tests or scratch code to prove a change locally, do not include them in the final pull request unless a maintainer asks for them.

## Review Expectations

We do review community contributions, but response times are best effort.

Small, low-risk, well-explained changes are the easiest for us to validate and merge. Large pull requests that span many concerns are much harder for a small team to take on.

## By Contributing

By submitting code, documentation, or other material to this repository, you agree that your contribution may be distributed under the MIT license used by this project.

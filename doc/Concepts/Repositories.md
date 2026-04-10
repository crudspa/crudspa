# Concepts | Repositories

Some application saves are simple. Many aren't. A screen that looks like one business record may actually coordinate shared contact details, user accounts, child collections, and feature-specific rows.

Crudspa uses repositories to keep that kind of persistence work cleaner. Publicly, the most useful way to think about them is as shared server-side helpers for persistence-heavy features.

## Crudspa's Stance

Repositories aren't the public application boundary. Services are.

Repositories usually exist to help with things like:

* shared entity persistence
* repeatable validation rules tied to a persisted shape
* reusable child-collection save behavior
* feature saves that touch more than one durable record

That keeps services from turning into giant blocks of repetitive persistence code.

## Where They Help

Repositories are especially useful when an application has concepts such as contacts, users, organizations, or reusable content parts that show up in many features.

In those cases, the framework's value isn't "you must always create repositories." The value is "you have a clean place for shared persistence logic when the application actually needs it."

## What They Are Not

Repositories are intentionally not:

* browser-facing APIs
* a replacement for service contracts
* the main place to start permission checks
* the final authority on feature-level tenancy

That distinction helps keep the public application surface clean and explicit.

## How To Use Them Well

Use a repository when you have persistence logic that's genuinely shared across features. Keep it close to persistence. Keep orchestration in the service. Keep feature-specific rules in the feature where they belong.

That's how the pattern stays helpful instead of becoming another abstraction layer added out of habit.

## Practical Guidance

When deciding whether to add a repository:

* add one when several features share the same persistence-heavy logic
* keep it server-side
* let services orchestrate the feature-level workflow
* don't let the repository become a hidden public API

## Tradeoffs

Repositories add another concept to the server side, so they are only worth it when they reduce real repetition or clarify real persistence work.

Crudspa's bias is pragmatic here. Use the pattern where it makes the application cleaner. Do not force it where a simple service method is already enough.

## Next Steps

* [Concepts | Services](Services.md)
* [Concepts | Tenancy](Tenancy.md)
* [Databases | Access](../Databases/Access.md)
* [Documentation Index](../ReadMe.md)

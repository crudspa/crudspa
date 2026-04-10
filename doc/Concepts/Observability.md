# Concepts | Observability

Good CRUD applications are operational software, not just UI. Teams need to know what failed, where it failed, and what to do next. That becomes even more important when some of the most important workflow runs in the browser.

Crudspa's observability stance is intentionally simple in public use: log through normal .NET APIs, let the application choose sinks, and keep browser and server events close enough that they can be understood as one story.

## Crudspa's Stance

The framework tries to make observability feel familiar:

* application code uses `ILogger`
* applications choose sinks such as console, Serilog, or Application Insights
* browser-side logs can be relayed back into the server-side logging pipeline
* cross-cutting telemetry belongs in wrappers, not scattered through feature code

That combination is one of the reasons the platform feels cleaner to work in than a stack that asks teams to maintain separate logging habits for the browser, the server, and background jobs.

## What This Means In Practice

For most application developers, the rule is easy:

* log normally
* return expected problems as structured results
* let the framework and the application host handle routing, aggregation, and sinks

You shouldn't need a second logging vocabulary just because a workflow started in a pane instead of on the server.

## Browser And Server Together

Crudspa's client logging support exists to preserve that simplicity. Browser-side logs can be relayed through the existing application boundary so they end up in the same general logging story as server-side events.

That helps because many real failures in CRUD+SPA applications are cross-boundary problems: a pane action starts in the browser, crosses a service boundary, touches the database, and then refreshes other users in real time. Operations teams need that story to stay coherent.

## Where To Extend

When teams want more than basic logging, the usual extension point isn't feature code. It's application configuration and wrapper behavior.

That's where you typically add:

* structured sink configuration
* distributed tracing
* timing metrics
* correlation data
* application-specific operational policy

Crudspa's public message here should be reassuring: the framework gives you a clean place to do this work without forcing every pane and service to become its own mini observability system.

## Background Work

The same story applies to jobs and worker processes. Background work is still part of the application's operational model, not a side system that should be invisible until it fails.

That's why the jobs sample sits in the same documentation story as the interactive applications.

## Practical Guidance

When building with Crudspa:

* use `ILogger` directly in application code
* keep sink decisions in the application
* keep tracing and policy in wrappers where appropriate
* treat browser logging as part of normal operations, not as a console-only afterthought

## Tradeoffs

This approach asks for a little architectural discipline, but it keeps the overall system much easier to operate. The payoff isn't cleverness. The payoff is that the application tells one clearer story when something goes wrong.

## Next Steps

* [Concepts | Exceptions](Exceptions.md)
* [Concepts | Wrappers](Wrappers.md)
* [Concepts | Services](Services.md)
* [Documentation Index](../ReadMe.md)

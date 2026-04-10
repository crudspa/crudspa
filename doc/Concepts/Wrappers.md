# Concepts | Wrappers

Most teams don't need to think about wrappers on day one. They become useful once an application grows enough that logging, authorization, retries, validation, tracing, or transaction policy would otherwise leak into every feature.

That's exactly why Crudspa has them. Wrappers are the framework's main seam for cross-cutting concerns. They let application code stay focused on the feature while the policy stays centralized and replaceable.

## Crudspa's Stance

Crudspa keeps different concerns at different boundaries:

* proxy wrappers for remote calls leaving the browser
* hub wrappers for SignalR entry points
* session wrappers for session and permission checks
* service wrappers for validation and server-side failure handling
* SQL wrappers for connection and transaction scope
* controller wrappers for HTTP endpoints

That separation is what keeps the framework from collapsing into one giant "pipeline" abstraction that means everything and therefore clarifies nothing.

## Why Application Developers Care

The biggest practical benefit is that feature code stays small.

A pane model can call a service without knowing how transport failures are translated. A hub can require permission without re-implementing the same access logic. A server service can focus on orchestration without re-writing the same validation and try/catch structure.

In other words, wrappers are part of why the public surface feels clean.

## When You Usually Touch Wrappers

Most teams touch wrappers in one of two cases:

* they want to understand where certain behavior already lives
* they want to customize a cross-cutting concern for their own application

Common examples include:

* adding tracing or metrics
* changing retry policy
* extending logging or observability
* centralizing custom authorization behavior

## Using Wrappers In Your Application

The common usage pattern is registration, not feature code.

```csharp
services.AddSingleton<IHubWrappers, HubWrappersCore>();
services.AddSingleton<IServiceWrappers, ServiceWrappersCore>();
services.AddSingleton<ISessionWrappers, SessionWrappersCore>();
services.AddSingleton<ISqlWrappers, SqlWrappersCore>();
```

If you need custom behavior, decorate or replace the implementation in your own application registration instead of pushing that policy into every pane, hub, or service.

That's the part worth emphasizing publicly: wrappers are how you customize cross-cutting behavior without making the rest of your application uglier.

## Relationship To Logging

Wrappers are a major part of the observability story, but they aren't the whole story.

Application code should still use `ILogger`. Browser log relay, host logging configuration, and wrapper-level tracing each solve different problems. Crudspa keeps those responsibilities adjacent, not tangled.

## Practical Guidance

When deciding whether something belongs in a wrapper:

* put it there if it applies broadly across many features
* keep it out if it's really feature-specific business behavior
* prefer decorating an existing wrapper over forking the whole policy surface
* keep the public service and pane APIs clean, even when the policy behind them grows

## Tradeoffs

Wrappers add a layer of indirection. That's real.

But for CRUD+SPA systems that need consistent policy across many screens and services, that indirection is far cheaper than letting every feature grow its own private version of the same plumbing.

## Next Steps

* [Concepts | Services](Services.md)
* [Concepts | Observability](Observability.md)
* [Concepts | Exceptions](Exceptions.md)
* [Documentation Index](../ReadMe.md)

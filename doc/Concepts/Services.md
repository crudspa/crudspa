# Concepts | Services

Crudspa works best when each layer has a clear job. Views render. Models coordinate the screen. Services expose capability. That sounds simple, but it's one of the biggest reasons Crudspa applications stay clean as they grow.

The framework's service guidance is practical rather than academic: remote calls should have a predictable shape, local helpers should stay simple, and application code shouldn't have to rediscover boundary design on every feature.

## Crudspa's Stance

Crudspa uses two broad service shapes:

* remote services, which cross the browser-server boundary and use `Request<T>` plus `Response<T>`
* local services, which stay inside one runtime and use ordinary method signatures

That split keeps the boundary predictable. It keeps trust boundaries obvious, gives expected failures a stable place to land, and keeps day-to-day application code from turning into transport ceremony.

## What You Usually Do As An App Developer

When you add a new feature, the normal path is:

1. Define a shared behavior contract in `Shared`.
2. Inject that contract into the pane model or component that needs it.
3. Return validation and business problems as structured errors.
4. Let the framework carry the call through the server boundary and back.

In practice, application code often feels as small as this:

```csharp
public async Task Save()
{
    var response = await WithWaiting("Saving...", () => _trackService.Save(new(Entity!)));

    if (response.Ok)
        ReadOnly = true;
}
```

That's the public-facing value of the service model. The pane model gets a clean call surface and a predictable result. The framework handles the rest.

## Remote Boundaries

`ITrackService` is a representative remote contract:

```csharp
public interface ITrackService
{
    Task<Response<IList<Track>>> FetchForPortal(Request<Portal> request);
    Task<Response<Track?>> Fetch(Request<Track> request);
    Task<Response<Track?>> Add(Request<Track> request);
    Task<Response> Save(Request<Track> request);
    Task<Response> Remove(Request<Track> request);
}
```

The important point isn't that every feature must look identical. It's that the browser and server agree on one small, readable shape. That makes services easier to use, easier to replace, and easier to reason about.

## Local Services

Not every service needs a remote boundary. Local helpers should stay simple.

That's why a service such as `IClickService` uses ordinary methods and events, and why application code logs through ordinary `ILogger` calls. Crudspa doesn't force request and response envelopes onto code that never leaves the current runtime.

## Why This Feels Clean In Practice

This service model buys a few practical wins:

* pane models stay focused on workflow instead of transport mechanics
* expected failures come back as data, not surprise exceptions
* cross-cutting concerns such as retries, logging, validation, and authorization have a clear home
* the same contract can support multiple application shapes

That's the deeper point. The framework is trying to make common application code feel calmer and more obvious, not more abstract.

## Practical Guidance

When adding or revising services:

* keep remote behavior contracts in `Shared`
* keep client-side proxies thin
* keep server-side orchestration in services, not in panes
* keep local-only helpers free of unnecessary RPC shape
* prefer explicit contracts over clever hidden coupling

## Tradeoffs

Crudspa's service model is more structured than pushing random browser calls at random endpoints. That structure is intentional.

It costs a little up-front discipline, but it pays back quickly once the application has enough screens, enough users, and enough change pressure that unclear boundaries become a daily drag.

## Next Steps

* [Concepts | Contracts](Contracts.md)
* [Concepts | Wrappers](Wrappers.md)
* [Concepts | Exceptions](Exceptions.md)
* [Documentation Index](../ReadMe.md)

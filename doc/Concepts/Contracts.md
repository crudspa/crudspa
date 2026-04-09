# Concepts | Contracts

CRUD+SPA systems become brittle when each layer invents its own shapes, names, and expectations. The UI drifts from the transport contract. The transport drifts from the server. Event payloads stop matching what listeners expect.

Crudspa avoids that drift by treating contracts as a first-class part of the architecture. Each module typically exposes a `Shared` project that holds the contracts both sides agree on. Those contracts are not incidental DTOs. They are the formal agreement for data, behavior, events, identifiers, and feature-specific configuration.

## Contract Families

Crudspa uses several contract families, each with a distinct job.

| Contract Family | Typical Folder | Purpose |
| --- | --- | --- |
| Behavior | `Contracts/Behavior` | defines callable service surfaces and shared interfaces |
| Data | `Contracts/Data` | defines DTOs, search models, and result models |
| Events | `Contracts/Events` | defines payloads sent through SignalR and the client event bus |
| Ids | `Contracts/Ids` | defines stable well-known IDs such as permissions and types |
| Config | `Contracts/Config` | defines structured JSON-backed plugin or job configuration |

The important idea is separation. A behavior contract says what can happen. A data contract says what shape that information has. An event contract says what changed. An IDs contract gives the system stable keys to agree on. A config contract lets plugins stay structured instead of falling back to loose JSON conventions.

## Behavior

`ITrackService` is a representative shared behavior contract:

```csharp
public interface ITrackService
{
    Task<Response<IList<Track>>> FetchForPortal(Request<Portal> request);
    Task<Response<Track?>> Fetch(Request<Track> request);
    Task<Response<Track?>> Add(Request<Track> request);
    Task<Response> Save(Request<Track> request);
    Task<Response> Remove(Request<Track> request);
    Task<Response> SaveOrder(Request<IList<Track>> request);
}
```

That interface is shared by:

* the client-side proxy
* the server-side implementation
* the hub methods that expose the service over SignalR
* the UI models that consume the behavior

The request and response envelopes are intentionally small:

```csharp
public class Request
{
    public Guid? SessionId { get; set; }
}

public class Request<T> : Request where T : class
{
    public T Value { get; set; } = null!;
}

public class Response
{
    public List<Error> Errors { get; set; } = [];
    public virtual Boolean Ok => Errors.Count == 0;
}

public class Response<T> : Response where T : class?
{
    public T? Value { get; set; }
}
```

That shape buys the framework several things:

* every remote call carries session context the same way
* validation and permission failures come back as data, not transport exceptions
* wrappers can add authorization, retry, logging, and transaction behavior around a stable surface
* services stay easy to proxy, observe, and replace

## Data

Crudspa's data contracts are meant to work directly in real UI flows. `PublisherContact` is a good public example because it carries a composite edit shape instead of one flat row:

```csharp
public class PublisherContact : Observable, IValidates, INamed, ICountable
{
    public String? Name => Contact.Name;

    public Guid? PublisherId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Contact Contact
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public User User
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            errors.AddRange(Contact.Validate());
            errors.AddRange(User.Validate());
        });
    }
}
```

Several important choices show up in that pattern:

* DTOs inherit from `Observable`, so they work directly with Blazor binding and change notification.
* DTOs commonly implement `IValidates`, so shared validation rules can run before a remote call.
* DTOs are often nested. One feature contract can carry a root entity plus related records that must move together.
* Most fields are nullable. Crudspa treats null as a real and useful value in data contracts instead of pretending the transport layer can encode every business rule by itself.

## Events

Event contracts describe what changed after a successful operation. They are intentionally small and focused.

`TrackEvents` is a representative event family:

```csharp
public class TrackPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class TrackAdded : TrackPayload;
public class TrackSaved : TrackPayload;
public class TrackRemoved : TrackPayload;
public class TracksReordered : TrackPayload;
```

The hub publishes those payloads as notices:

```csharp
public class Notice<T> : Notice where T : class
{
    public Notice(T payload)
    {
        Payload = payload.ToJson();

        var type = payload.GetType();
        Type = $"{type.FullName}, {type.Assembly.GetName().Name}";

        Posted = DateTimeOffset.Now;
    }
}
```

On the client, the event bus dispatches the payload to any subscriber implementing `IHandle<T>`.

That separation keeps behavior contracts clean. A save call returns whether the save succeeded. A saved event tells other interested screens that they should refresh. Those are different responsibilities, so they use different contracts.

## Organizing Contracts

A healthy Crudspa module keeps contracts near the top of the dependency graph:

* `Shared` owns the contracts
* `Client` depends on `Shared`
* `Server` depends on `Shared`
* models, hubs, services, and repositories all build on those same shared definitions

Some modules also reuse canonical contracts from adjacent modules when the vocabulary is already shared. `Content.Design`, for example, works with `Track` and `Portal` contracts that already live in the display-side shared layer.

## Tradeoffs

Contract-centric development asks for discipline. Teams have to think clearly about shape and naming before they start wiring behavior together.

In exchange, the system becomes easier to extend, easier to reason about, and much harder to accidentally couple through hidden assumptions.

## Next Steps

* [Concepts | Services](Services.md)
* [Concepts | Plugins](Plugins.md)
* [Concepts | Sessions](Sessions.md)
* [Documentation Index](../ReadMe.md)

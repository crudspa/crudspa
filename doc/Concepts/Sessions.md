# Concepts | Sessions

The word `session` is overloaded in .NET web development. Many developers hear it and think of per-server memory bags, `HttpContext.Session`, or a place to stash arbitrary UI state.

Crudspa means something different. A Crudspa session is a durable, database-backed security and navigation record for one client relationship to one portal. It may begin anonymously, later become associated with a user, and then drive permissions, navigation, and real-time subscriptions across the application.

That distinction is important. If you understand Crudspa sessions, you understand a large part of how the framework hangs together.

## What A Session Contains

The shared session contract is small but powerful.

```csharp
public class Session
{
    public Guid? Id { get; set; }
    public User? User { get; set; }
    public Guid? PortalId { get; set; }
    public ObservableCollection<Guid> Permissions { get; set; } = [];
    public ObservableCollection<NavSegment> Segments { get; set; } = [];
    public ObservableCollection<Screen> Screens { get; set; } = [];
}
```

A Crudspa session is not only an authentication token. It is also the carrier for:

* the current portal
* the current user, if any
* the user's effective permission IDs for that portal
* the allowed navigation tree for that session
* any currently open screens the client wants to preserve

That is why the concept shows up everywhere in the framework.

## Lifecycle

### 1. Anonymous Session Creation

When the client starts, it asks for session state. The server either fetches the current session or creates a new anonymous one.

```csharp
public async Task<Response<Session?>> FetchOrCreate(Request<Session> request)
{
    if (session.Id.HasSomething())
    {
        var selected = await sessionFetcher.Fetch(session.Id);

        if (selected is not null)
            return selected;
    }

    return await sqlWrappers.WithConnection(async (connection, transaction) =>
    {
        var sessionId = cryptographyService.GetRandomGuid();
        await SessionInsert.Execute(connection, transaction, PortalId, sessionId);
        return await sessionFetcher.Fetch(sessionId);
    });
}
```

`FrameworkCore.SessionInsert` writes a row immediately. This is a database-backed session, not an in-memory one.

### 2. Client Initialization

On the client, `SessionStateCore` fetches the session, stores the session cookie when appropriate, and tells the proxy wrapper which `SessionId` to attach to future RPC calls.

### 3. Authentication

When the user completes authentication, the server attaches that user to the existing session instead of minting a whole new application concept.

```sql
update [Framework].[Session]
set UserId = @UserId
    ,UserAdded = @now
where Id = @Id
```

That means the same session record evolves from anonymous to authenticated.

### 4. Permission And Navigation Resolution

Session fetch does more than load the bare session row. It also resolves the user's permissions and allowed segments for the current portal.

`SessionStateServiceSql` shows the pattern:

```csharp
public async Task<Response<Session?>> Fetch(Request<Session> request)
{
    var response = await sessionService.FetchOrCreate(request);

    if (!response.Ok || response.Value is null)
        return response;

    var session = response.Value;
    var segments = await segmentFetcher.Fetch(session.Id, session.PortalId);

    session.Segments = segments.ToObservable();

    return response;
}
```

### 5. End Or Expire

Signing out marks the session ended and deleted in the database.

```sql
update [Framework].[Session]
set  IsDeleted = 1
    ,Ended = @now
where Id = @Id
    and IsDeleted = 0
```

Background jobs can also expire sessions in bulk, and the session cache can be invalidated across the host when shared security data changes.

## Session-Driven Navigation

One of the most distinctive parts of Crudspa is that the session drives the navigation tree.

After `SessionStateCore` refreshes the session, `NavigatorCore` rebuilds its registrations and screens from `Session.Segments`. That means:

* navigation is already filtered for the current session
* portal-specific structure arrives as data
* reopening or bouncing the shell can restore the right screen tree for that session

This is very different from a router that simply exposes every route and waits to deny access later.

## Session Persistence

Portals decide whether sessions should persist through browser restarts with `Portal.SessionsPersist`.

`SessionStateCore.Initialize` uses that setting to choose whether the `SessionId` cookie gets a long expiration or a session-only lifetime.

That gives each portal an explicit persistence policy instead of assuming one default for the whole system.

## This Is Not ASP.NET Session State

A Crudspa session is not a general-purpose key-value bag for arbitrary server memory.

It is:

* durable
* database-backed
* security-relevant
* shared across hub and controller flows
* part of how navigation and permissions are resolved

If an application genuinely needs extra session-scoped state, the usual Crudspa advice is to create a normal table keyed by `SessionId`. That keeps the data explicit, queryable, and compatible with the rest of the framework's architecture.

## Sessions And Real-Time Behavior

The session also anchors real-time subscriptions.

* the client proxy subscribes the current connection after setting `SessionId`
* the server resolves the session and adds the connection to organization or permission groups
* notifications are then scoped to the audience implied by that session

So the session is not just about sign-in. It is part of the framework's messaging model too.

The important point is that the same session concept spans startup, authentication, navigation, and notice audience membership. A session can begin anonymously, pick up navigation and access state, attach a user and permissions after sign-in, drive SignalR group membership, and then end explicitly or by expiration.

## Tradeoffs

Crudspa sessions carry more responsibility than the simple cookies many web apps start with. That means session design affects navigation, security, and real-time behavior all at once.

The payoff is coherence. One durable concept ties together the browser, the hub, the server, and the database in a way that is easy to reason about.

## Next Steps

* [Concepts | Security](Security.md)
* [Concepts | Tenancy](Tenancy.md)
* [Concepts | Services](Services.md)
* [Documentation Index](../ReadMe.md)

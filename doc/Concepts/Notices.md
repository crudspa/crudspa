# Concepts | Notices

Multi-user CRUD screens go stale quickly if the architecture only thinks in request and response pairs. One user saves a record. Another user still sees the old value. Lists drift. Counts drift. Runtime content can drift too.

Crudspa treats that as a first-class problem. Its notice model isn't an optional add-on. It's part of the platform's default story for keeping connected clients fresh.

`Catalog` is the smallest place to study the notice loop end to end. `Composer` plus `Consumer` then shows how the same event vocabulary crosses host boundaries for authored content, and `Samples/Jobs/Engine` plus `Catalog` or `Composer` shows the same pattern for background work.

## Default Flow

The normal notice flow is simple:

1. a client sends a save, add, remove, or other mutation request
2. the hub applies permission and session wrappers
3. the server service performs the mutation
4. after success, the host publishes a typed notice, and it can also publish a typed gateway event if another host needs to react
5. subscribed clients receive the notice
6. client models refresh, replace, or remove local state as needed

The important detail is step four. Crudspa publishes notices after a successful server-side change, not as a guess from the client.

In practice, a notice flow starts with a client mutation, passes through hub wrappers and a server service, and only publishes after the save succeeds. Subscribed clients then refresh, replace, or remove local state. If another host also needs to react, the same event can take a gateway hop before that host rebroadcasts its own notice. The key boundary is still the save itself. Crudspa publishes notices after the server accepts the change, not before.

## Across Hosts

Crudspa doesn't treat cross-host refresh as a different system. It keeps the same typed event vocabulary and adds a gateway hop when another host needs to invalidate caches or rebroadcast a notice.

In the shipped samples, `GatewayServiceEventGrid` uses the checked-in `EventReceiverUrls` values to post Event Grid-shaped payloads to the sample controllers:

* `Composer` publishes `PageContentChanged` and `PortalRunChanged` so `Consumer` can invalidate runtime content caches, warm themed output, and rebroadcast.
* `Samples/Jobs/Engine` publishes job and schedule events so `Catalog` and `Composer` can refresh their `Jobs` and `Schedules` panes.

That's still one architectural loop, not two unrelated systems. The gateway carries the event between hosts, then each host uses the normal notice mechanism for its own connected clients.

## Subscription And Audience Model

Subscriptions happen in the context of a real session. That helps because audience scoping depends on who the user is and what they are allowed to see.

Crudspa's hub subscription model groups connections by scopes such as:

* organization plus permission, for shared operational updates
* organization plus contact, for user-specific updates

That gives the platform a practical way to keep notices useful without turning them into global broadcasts. An administrative save can notify the right audience. A user-specific runtime event can stay personal.

## Notice Payloads And Client Handling

A notice usually carries a typed event payload rather than a full replacement object. That keeps the transport small and encourages clients to refetch the authoritative record when necessary.

On the client side, the event bus and screen models do the rest. Common update patterns include:

* refresh a currently open edit pane
* replace one card or form inside a list or many surface
* remove an item that was deleted elsewhere
* ignore the event if it's outside the current scope

Those scope-aware helpers are a big reason the notice flow stays manageable. Panes aren't all inventing their own synchronization logic.

## Security And Reliability

Notices should never become a shortcut around the normal security model.

Crudspa keeps the trust boundary on the server:

* permissions are checked before the mutation succeeds
* notices are scoped to subscribed audiences
* client refreshes still go back through the normal service boundary

This is also better for reliability. If a client misses a notice or reconnects later, the recovery path is still a normal fetch, not a fragile stream of assumed state.

## Practical Guidance

When adding notice behavior to a feature:

* publish notices only after successful commits
* keep payloads small and typed
* let the client refetch authoritative data when it needs to
* use scope checks so unrelated panes don't churn
* handle reconnects by resubscribing and refreshing where appropriate

This keeps the architecture honest and keeps the event model from becoming a second, hidden data layer.

## Tradeoffs

Notice-driven refresh adds moving parts. You are now thinking about sessions, audience scope, reconnects, and eventual refresh behavior. Cross-host refresh adds gateway endpoints and cache invalidation on top of that. That's more work than a static page.

But for serious CRUD+SPA applications, it's usually the right work. The alternative is stale screens and quiet user confusion.

## Next Steps

* [Concepts | Services](Services.md)
* [Patterns | Edit](../Patterns/Edit.md)
* [Patterns | Many](../Patterns/Many.md)
* [Documentation Index](../ReadMe.md)

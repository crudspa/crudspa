# Concepts | Security

Security in a CRUD+SPA application is never one switch. Authentication, session state, navigation filtering, service boundaries, file access, and data rules all have to agree or the application eventually leaks behavior.

Crudspa treats security as a layered application concern. That's important for teams evaluating the framework because the public story isn't "trust the UI and hope for the best." The story is "keep the rules consistent from shell to database."

## Crudspa's Stance

Crudspa's default security model combines:

* session-first authentication
* permission-aware navigation
* permission checks at the application boundary
* final enforcement close to the data
* explicit auditing of denied access

That stack is meant to make rich applications feel safer to grow, especially once there are many panes, many permissions, and many related entities.

## What Application Developers Usually See

Most application developers meet security in three places:

* the sign-in flow
* the set of panes or actions a user is allowed to see
* the service calls that succeed or fail based on the current session and permissions

That's a healthier public model than making every team invent its own mix of hidden routes, scattered claims checks, and UI-only restrictions.

## Authentication And Sessions

Crudspa authentication is session-first. A session can start anonymously, pick up a user later, and then shape navigation, permissions, and notices for the rest of the application.

That gives the application one consistent security anchor instead of a pile of unrelated checks.

## Permissions And Navigation

Crudspa doesn't wait until a user clicks something to think about permissions. Navigation can already be filtered by the current session, so users are guided toward the parts of the application that actually belong to them.

That doesn't replace server-side checks. It complements them. A hidden pane is helpful. A denied service call is still required. Final enforcement still has to happen.

## Service And Data Enforcement

At the service boundary, Crudspa expects explicit permission checks. At the data layer, it expects the final scope rule to survive there as well.

You'll hit cases where the browser is wrong, stale, or bypassed in real applications. Security still has to hold.

## Auditing

Denied access isn't just a UX event. It's an operational event.

Crudspa treats access denial as something worth recording so teams can understand repeated problems, misconfiguration, or genuinely suspicious behavior.

## Practical Guidance

When building with Crudspa:

* treat sessions as the application's security anchor
* filter navigation where it helps, but don't stop there
* enforce permission checks at the application boundary
* keep the final row-level rule close to the data
* treat denied access as security-relevant even when it's expected

## Tradeoffs

This model is more deliberate than relying only on UI hiding or only on middleware claims. The tradeoff is a little more structure. The payoff is a security story that holds up much better once the application becomes deep, stateful, and multi-user.

## Next Steps

* [Concepts | Sessions](Sessions.md)
* [Concepts | Tenancy](Tenancy.md)
* [Concepts | Exceptions](Exceptions.md)
* [Documentation Index](../ReadMe.md)

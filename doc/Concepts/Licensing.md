# Concepts | Licensing

Licensing in Crudspa is an entitlement boundary shared by content and domain modules. A license is not the same thing as authentication or a portal permission: authentication identifies the session, permissions authorize application capabilities, and licenses describe which published experiences the session is entitled to use.

## License Relationships

The publisher module defines the license record and the services that relate licenses to publishable content. The current source includes relationships for:

* assessments
* blogs
* campaigns
* forums
* segments
* surveys
* tracks
* units

Each relationship follows the normal Crudspa node shape with shared contracts, a client proxy, a server hub and service, events, and SQL-backed persistence.

## Runtime Resolution

Runtime modules should not depend directly on one publisher product model. `Content.Display` therefore asks `ISessionLicenseResolver` for the license IDs associated with the current session.

Hosts choose the implementation:

* `SessionLicenseResolverNone` intentionally resolves no licenses and is useful in focused samples or hosts that expose only public content.
* `SessionLicenseResolverPublisherSql` resolves publisher licenses for applications that compose the education publisher module.
* another application can supply its own resolver behind the same interface.

Forum reads, writes, reactions, and media fetches all pass the resolved license IDs into SQL enforcement. A `LicensedUsers` forum is therefore protected even if a client calls the service contract directly.

## Licensing And Messaging

Campaigns also carry selected licenses. The publisher module exposes campaign-license administration alongside the equivalent forum, blog, assessment, survey, segment, track, and unit relationships.

That shared vocabulary lets a broader application manage content entitlements in one place while the reusable Content modules stay focused on authoring, runtime delivery, and messaging behavior.

## Practical Guidance

When adding licensed content to a host:

* register exactly one `ISessionLicenseResolver` on the runtime server
* return only licenses belonging to the current authenticated session and application scope
* keep public access explicit through content access modes rather than treating a missing resolver as a security shortcut
* enforce license relationships again in SQL-backed reads and writes
* test anonymous, signed-in but unlicensed, and licensed sessions separately

The focused Composer and Consumer samples register `SessionLicenseResolverNone`, so their seeded `Everyone` forums remain publicly readable. Use the publisher-backed resolver or your own implementation when you want to demonstrate licensed runtime content.

## Next Steps

* [Concepts | Forums](Forums.md)
* [Concepts | Messaging](Messaging.md)
* [Concepts | Security](Security.md)
* [Concepts | Sessions](Sessions.md)
* [Documentation Index](../ReadMe.md)

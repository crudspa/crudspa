# Concepts | Injection

Crudspa is meant to be composed, not copied and patched. That only works if dependencies are visible, replaceable, and easy to follow.

Dependency injection is the framework's answer. It keeps application choices explicit and makes it practical to swap providers, extend policies, and grow a solution without rewriting the whole platform around each new requirement.

## Crudspa's Stance

The guiding rule is simple: keep composition explicit.

Application code should be able to answer:

* what services are we using?
* what did we choose to override?
* where do infrastructure decisions live?

Crudspa expects those answers to be obvious from application registration rather than hidden inside scattered constructors and static helpers.

## What This Looks Like In Practice

Applications usually register the framework pieces they want, then add their own modules and providers on top.

```csharp
services.AddSingleton<IAuthService, AuthServiceTcpEmailTfa>();
services.AddSingleton<IProxyWrappers, ProxyWrappersCore>();
services.AddSingleton<ISessionState, SessionStateCore>();
services.AddSingleton<IPortalRunService, PortalRunServiceTcp>();
services.AddSingleton<ISegmentService, SegmentServiceTcp>();
```

The same idea holds on the server side, where applications choose wrappers, repositories, infrastructure providers, and service implementations in one place.

## Why This Helps Consumers

That helps because Crudspa isn't trying to own your whole application. It's trying to give your application a strong default shape.

Clear registration keeps it easy to:

* add modules in layers
* replace infrastructure providers
* keep custom application behavior separate from reusable framework behavior
* understand what an application actually depends on

That's why this topic belongs in the public docs. It's not just an implementation detail. It's part of how teams use Crudspa cleanly.

## Models And DI

Crudspa doesn't force every object into the container.

Shared services and infrastructure usually live in DI. Short-lived screen models are often created in components with injected services passed in. That balance keeps application code straightforward: DI owns long-lived capability, while components own local workflow state.

## Practical Guidance

When composing your own application:

* keep application registration centralized
* prefer interface-based replacement over conditionals spread through feature code
* keep request-specific mutable state out of singleton services
* treat registration as part of the architecture, not as startup noise

If you want the broader application picture, read [Overview | Applications](../Overview/Applications.md).

## Tradeoffs

Dependency injection can become noisy when it's used carelessly. Crudspa tries to avoid that by keeping registration predictable and by giving each layer a clear role.

The result is a codebase that stays easier to reshape as the application grows.

## Next Steps

* [Overview | Applications](../Overview/Applications.md)
* [Concepts | Services](Services.md)
* [Concepts | Wrappers](Wrappers.md)
* [Documentation Index](../ReadMe.md)

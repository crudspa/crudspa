# Concepts | Exceptions

One of the easiest ways to make a CRUD application feel fragile is to use exceptions for everyday user flow. A missing field, a denied permission, or a validation problem should not behave like a system fault.

Crudspa takes a calmer approach. Expected problems come back as structured results. Exceptions are reserved for defects, broken assumptions, and real infrastructure failures.

## Crudspa's Stance

Crudspa separates three kinds of failure:

* user-correctable problems, which should come back as `Response.Errors`
* normal denials, such as permission failures, which should also come back safely
* genuine faults, which should be logged, contained, and treated as problems to fix

That is not just an implementation preference. It changes how application code feels to write and maintain.

## What This Looks Like In Application Code

A pane model can treat most failure as part of normal workflow:

```csharp
public async Task Save()
{
    var response = await WithWaiting("Saving...", () => _trackService.Save(new(Entity!)));

    if (response.Ok)
        ReadOnly = true;
}
```

That is the important public-facing benefit. The application does not need to turn every failed save into a crash path. It can stay in the normal screen workflow and show useful feedback.

## Where Exceptions Still Matter

Crudspa still expects exceptions to exist. They just mean something different.

In practice, exceptions are still appropriate for:

* broken configuration
* unsupported states
* invalid type metadata
* database and transport faults
* defects in application or framework code

When those happen, the framework aims to contain them at the right boundary and log them clearly.

## Why This Helps

This policy improves several things at once:

* user-facing flows become more stable
* operational failures become easier to spot because they are not mixed with routine validation
* application code stays cleaner because it works with results instead of defensive exception choreography

That is a big part of why Crudspa applications feel production-minded rather than improvised.

## Practical Guidance

When building on Crudspa:

* return `Error` objects for expected, user-correctable problems
* reserve `throw` for conditions that really are exceptional
* treat logged exceptions as work to follow up on, not background noise
* keep failure handling close to the appropriate boundary instead of scattering it through panes

## Tradeoffs

This approach asks teams to model failure a little more explicitly. That is real work.

But the payoff is a calmer system: fewer surprise crashes, clearer logs, and application code that reads more like workflow and less like damage control.

## Next Steps

* [Concepts | Observability](Observability.md)
* [Concepts | Services](Services.md)
* [Concepts | Security](Security.md)
* [Documentation Index](../ReadMe.md)

# Concepts | Plugins

Crudspa is built for applications that need many screens, varied workflows, and room to grow without rewriting the shell every time. Plugins are a big part of how it does that cleanly.

In public terms, the idea is straightforward: your application supplies pane, navigation, report, and display components, and Crudspa loads the right one based on metadata instead of hard-coded branching.

## Why This Matters

For application developers, plugins solve a very practical problem. They let you add new work surfaces without turning the shell into a long list of special cases.

That means:

* your application can stay modular
* different applications can share the same platform vocabulary
* the shell stays reusable even as your screens diverge

## The Public Shape

Most teams first meet the plugin model through pane components.

`TrackEdit` is a representative public example:

```csharp
public partial class TrackEdit : IPaneDisplay, IDisposable
{
    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public ITrackService TrackService { get; set; } = null!;
}
```

That is the part application authors need to care about most. You implement the expected plugin interface, build the screen you need, and let the framework host it inside the shell.

## Where Plugins Show Up

Crudspa uses the same general idea in several places:

* navigation layouts
* segment displays
* pane displays and editors
* reports
* content binders and elements
* style-related editors and displays

The mechanism is consistent on purpose. Once you understand the plugin model in one place, the rest of the platform becomes easier to navigate.

## Using Plugins Well

When adding a new plugin:

* keep the component focused on one work surface
* honor the interface the host expects
* use metadata to select the plugin instead of adding shell-specific branching
* keep application-specific screens in your application or module code, not in framework core

That is how Crudspa keeps customization feeling clean instead of improvised.

## What Crudspa Handles For You

Crudspa handles the dynamic resolution, parameter passing, and shell integration. You do not need to build your own plugin loader just to add a pane or swap a navigation surface.

That is the main public value of the plugin model: flexibility without shell chaos.

## Tradeoffs

Plugin systems move some mistakes from compile time to runtime. Crudspa accepts that tradeoff because the gain in flexibility is large for the kind of applications it targets.

The framework reduces the risk by keeping the plugin contracts small and explicit.

## Next Steps

* [Concepts | Navigation](Navigation.md)
* [Concepts | Models](Models.md)
* [Components | Overview](../Components/ReadMe.md)
* [Documentation Index](../ReadMe.md)

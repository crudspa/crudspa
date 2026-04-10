# Concepts | Navigation

Navigation is where many CRUD+SPA systems reveal their limits. Traditional routing is often too page-oriented. Browser support gets weaker as the UI becomes richer. Deep links become awkward. Multi-pane workflows become brittle.

Crudspa treats navigation as one of the platform's main differentiators. The shell isn't a thin route table. It's a structured, data-driven application model.

## Canonical Terms

These terms are worth learning:

* a `portal` is the top-level shell for an application experience
* a `segment` is a navigable branch inside that portal
* a `pane` is one work surface rendered inside a segment
* a `path` is the URL-resolved address to the current shell state
* a `node` is the feature boundary behind a pane or related pane family

If you keep those meanings stable, the rest of the shell becomes much easier to reason about.

## Shell Model

The portal owns the broadest navigation concerns:

* branding
* session persistence behavior
* whether sign-in is allowed or required
* which navigation display plugin should render the shell

Inside the portal, segments define structure. A segment can be fixed, hierarchical, permission-aware, and capable of containing both child segments and panes.

Panes are where users actually work. A pane might host an edit surface, a list, a report, a runtime content screen, or a design surface. The important part is that the shell treats each one as a first-class, deep-linkable unit.

In practical terms, a portal contains segments, segments can contain child segments and panes, and the current path resolves to one active pane. Desktop and mobile layouts may render that structure differently, but the underlying shell model stays the same.

## Routing And URL State

Crudspa keeps normal browser expectations intact:

* paths are deep-linkable
* the back button still needs to work
* query strings can carry pane-adjacent UI state such as the active tab

This is one of the reasons the `Navigator` service is so central. It's not just moving between pages. It's keeping the shell, the URL, and local UI state aligned.

The platform also handles pane lifecycle concepts such as whether a pane is new, whether it has already been loaded, and which configuration payload should be applied when it opens.

## Plugin Resolution

Navigation is plugin-driven at multiple levels.

* the portal selects a navigation display plugin
* a segment selects a segment display plugin
* a pane selects its pane display plugin

That means the shell stays generic while the experience stays flexible. An admin portal, a consumer portal, and a content-first site can all reuse the same platform vocabulary while rendering very different shells.

## Sessions, Authentication, And Permissions

Navigation isn't separate from security.

Portal rules can require sign-in before the shell becomes available. Segment and pane metadata can carry permission requirements. Session state influences what the user can load, which notices they can subscribe to, and which work surfaces should even appear.

That's a healthier model than treating security as a late filter on an otherwise public route table.

## Practical Guidance

When adding a new pane or navigation shape:

* start by deciding which portal and segment it belongs to
* keep the pane focused on one feature boundary
* choose a display type that matches the user workflow instead of forcing every feature into one shell layout
* prefer plugin selection through metadata over hard-coded branching in the shell
* let query strings hold small UI state, not whole application state

This tends to produce shells that stay flexible as the product grows.

## Tradeoffs

Crudspa's navigation model asks you to learn a little more vocabulary up front than a simple page router would. Metadata-driven plugin selection also means some shell mistakes appear at runtime rather than compile time.

The payoff is substantial. You get deep linking, browser support, multi-pane workflows, and highly reusable shell behavior without flattening everything into a page list.

## Next Steps

* [Overview | Vocabulary](../Overview/Vocabulary.md)
* [Concepts | Plugins](Plugins.md)
* [Concepts | Models](Models.md)
* [Documentation Index](../ReadMe.md)

# Styling | Stylesheets

Most Crudspa developers don't need to work at the raw CSS level very often. The component layer already applies most of the framework classes for forms, buttons, toolbars, tabs, dialogs, trees, cards, and navigation shells.

This page is still important. It's the single reference for the Crudspa stylesheet system itself: how stylesheet entrypoints are composed, how module layering works, how host-specific SCSS fits in, and how the framework exposes theme tokens to those stylesheets. If you need to extend the platform cleanly or build a custom style stack that still feels like Crudspa, this is the page to read.

If you mainly want to build screens, start with [Components | Layouts](../Components/Layouts.md) and [Components | Forms](../Components/Forms.md). This page is the map underneath those higher-level APIs.

The key distinction is compile time versus runtime. Structural SCSS entrypoints are bundled into the host stylesheet ahead of time, while theme tokens are applied later as runtime values for a specific portal or site.

## Layering

Crudspa assembles each host stylesheet from explicit SCSS entrypoints. Each style-contributing project can expose a stylesheet entrypoint, and host applications compose those entrypoints into a final host stylesheet.

A real stack looks like this:

```scss
// Framework/Core/Server/Styles/framework-core.scss
@import "normalize";
@import "symbols";
@import "defaults";
@import "html";
@import "animation";
@import "borders";
@import "buttons";
@import "components";
@import "fields";
@import "flexbox";
@import "helpers";
@import "input";
@import "menus";
@import "navigation";
@import "sizing";
@import "spacing";
@import "text";
@import "windows";
@import "Radzen/crudspa";
```

```scss
// Content/Display/Server/Styles/content-display.scss
@import "framework-core";
```

```scss
// Education/Common/Server/Styles/education-common.scss
@import "content-display";
```

```scss
// Portals/Provider/Server/Styles/portals-provider.scss
@import "content-design";
@import "education-common";
```

That separation is deliberate:

* module stylesheets express reusable style layering
* host stylesheets express the final application shell and any host-specific styles
* the runtime theme token sheet stays separate from the structural stylesheet

Crudspa's own layout and spacing system is native. Optional vendor styles such as Radzen or Telerik remain overlays, not the foundation.

## Why This Structure Works

Stylesheet composition lives in SCSS entrypoints, where Sass developers expect it. That keeps the layering visible in the code that owns the styling instead of in a separate manifest format.

This structure also matches the way the platform is organized in C#. Shared framework styling lives in the lower framework stylesheets. Content and domain modules build on those stylesheets. Hosts then compose the final stylesheet that matches their own application shell.

The result is easier to read and easier to maintain:

* structural styling stays with the module or host that owns it
* stylesheet order is expressed directly in Sass
* host stylesheet entrypoints stay small and declarative
* theme data can be applied separately from stylesheet structure

## Defaults Contract

Crudspa uses a single `defaults.scss` file as the shared defaults contract.

That file serves two jobs:

* it defines the framework's default and derived Sass values
* it gives the runtime theme pipeline a single place to read the token catalog

In practice, that means `defaults.scss` is where Crudspa defines the platform's default token values and derived values that should move with them. For example, it can define hover colors, muted colors, and vendor-facing semantic values in one place.

When the host stylesheet is compiled for runtime theming, those same variables can resolve through browser-native CSS custom properties such as `var(--button-background-color)`. That lets the active portal theme flow through without rebuilding the structural stylesheet for every portal revision.

This gives Crudspa a clean contract:

* Sass owns structural composition and shared defaults
* runtime theming owns actual portal-specific token values
* preview systems can scope tokens without recompiling the entire stylesheet

## Module Inheritance In SCSS

The stylesheet entrypoint system mirrors the way the C# projects already relate.

`Content.Display` sits on top of `Framework.Core`, so its stylesheet entrypoint consumes the framework stylesheet. `Content.Design` sits one layer higher, so its stylesheet consumes `Content.Display`. Education modules can then consume the content stylesheets and add their own domain-specific SCSS.

This is the key idea: SCSS layering follows the same architectural shape as the platform itself.

That's a big win for maintainability because developers can reason about styling with the same mental model they already use for client, shared, and server code.

## Naming

The naming style is intentionally closer to a UI framework than a utility-first micro-language.

* Framework classes use the `c-` prefix.
* Sizes are semantic words such as `tiny`, `medium`, `wide`, and `max`, not raw numeric scales.
* Modifier classes are usually plain words such as `top`, `right`, `selected`, `primary`, `tight`, or `hidden`.
* Feature-specific application classes usually use their own prefixes, such as `cec-` or `ces-`.
* Stylesheet entrypoint files use the `area-module.scss` pattern where that helps developers find the right file quickly, for example `portals-provider.scss` or `sites-summer.scss`.

That difference is important. Crudspa isn't trying to turn every page into a long string of tiny numeric utility tokens. The goal is a readable, shared language that matches the kinds of screens CRUD developers build every day.

## Responsive Foundation

The framework's responsive behavior starts from the defaults contract.

### Breakpoints

| Name | Default Value |
| --- | --- |
| `screenMin` | `10em` |
| `screenTiny` | `20em` |
| `screenSmall` | `30em` |
| `screenMedium` | `40em` |
| `screenLarge` | `50em` |
| `screenWide` | `60em` |
| `screenMax` | `75em` |

These breakpoints are intentionally compile-time Sass values because they are used heavily in `@media` rules.

### Root Font Scaling

The `html` element changes font size across those same breakpoints. By default it moves from `0.7em` at the smallest screens up to `0.95em` at the largest. Because much of Crudspa uses `em` and `rem`-based measurements, text and layout scale together.

### Spacing Scale

The spacing utilities also change with screen size.

| Size | Typical Range |
| --- | --- |
| `tight` | `0` |
| `micro` | about `0.125rem` to `0.25rem` |
| `tiny` | about `0.125rem` to `0.5rem` |
| `small` | about `0.25rem` to `1rem` |
| `medium` | about `0.5rem` to `2rem` |
| `large` | about `1rem` to `4rem` |

That scaling is a major reason Crudspa layouts hold up on phones without every page author hand-tuning spacing rules.

## Class Families

You don't need to memorize every class. Learn the families.

### Layout

| Class Family | Examples | Purpose |
| --- | --- | --- |
| Horizontal wraps | `c-wrap`, `c-nowrap`, `c-nowrap-top`, `c-nowrap-baseline` | Build horizontal flex rows, with wrapping enabled by default |
| Vertical stacks | `c-stack`, `c-stretched-vertically` | Build vertical layouts and full-height panels |
| Flexible fill | `c-star` | Take remaining width in a row |
| Centering | `c-centered-vertically-container` | Center content both ways |
| Split panes | `c-two-panes` | Collapse from side-by-side into stacked layout on smaller screens |

These classes are the foundation under `Wrapped`, `Stacked`, `Toolbar`, `ButtonGroup`, and several other components.

### Spacing

Crudspa uses a consistent naming pattern for spacing:

* `c-padding-{size}`
* `c-padding-{size}-{direction}`
* `c-margin-{size}`
* `c-margin-{size}-{direction}`

Directions include `top`, `right`, `bottom`, `left`, `hor`, and `vert`.

### Sizing

The sizing scale is central to Crudspa forms because `Field` maps directly to it.

| Class | Default Width |
| --- | --- |
| `c-pico` | `2.5em` |
| `c-nano` | `4em` |
| `c-micro` | `6em` |
| `c-min` | `8em` |
| `c-tinier` | `10em` |
| `c-tiny` | `12em` |
| `c-small` | `16em` |
| `c-medium-small` | `18em` |
| `c-medium` | `20em` |
| `c-medium-large` | `24em` |
| `c-large` | `28em` |
| `c-larger` | `34em` |
| `c-wide` | `40em` |
| `c-max` | `52em` |

### Buttons, Components, And Shells

The practical class families stay the same even though the assembly mechanism changed.

* buttons still center on semantic variants such as `save`, `cancel`, `destroy`, and `primary`
* feedback surfaces still center on alerts, waiters, cards, filters, and toolbars
* content surfaces still center on binders, pages, and embedded reports
* shell surfaces still center on navigation, windows, tabs, modals, and footers

The important change isn't the class vocabulary. The important change is that those classes compile from explicit stylesheet entrypoints and consume runtime theme tokens through `defaults.scss`.

## Practical Guidance

* Add reusable framework CSS in the stylesheet layer where future developers would expect to find it.
* Add domain or host-specific SCSS in that module or host's own stylesheet entrypoint.
* Treat `defaults.scss` as the single Sass source of truth for themeable values and derived defaults.
* Keep host stylesheet entrypoints small and declarative. They should mostly say which modules they compose.

## Tradeoffs

The stylesheet-entrypoint model is conventional and easy to reason about, but it does ask teams to be disciplined about where style logic lives.

If a value is part of the shared theme contract, it belongs in `defaults.scss` and the runtime theme pipeline. If a rule is structural or host-specific, it belongs in a stylesheet entrypoint or a host stylesheet. Keeping those jobs separate is what makes the system predictable.

## Next Steps

* [Styling | Theming](Theming.md)
* [Styling | Layouts](Layouts.md)
* [Components | Layouts](../Components/Layouts.md)
* [Components | Forms](../Components/Forms.md)
* [Documentation Index](../ReadMe.md)

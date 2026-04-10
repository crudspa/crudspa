# Styling | Theming

Many web applications bake their theme into a deploy-time stylesheet and stop there. If you want a new color system, new fonts, or a different visual rhythm, you rebuild and redeploy.

Crudspa keeps host stylesheets structural and cacheable, while portal themes are runtime token sheets generated from structured data. That means a site can be substantially re-skinned without recompiling the whole stylesheet for each portal, and preview systems can stay self-contained while still using the real theme contract.

Crudspa theming works because structural CSS, runtime theme tokens, and page-level or box-level overrides stay separate. The host keeps its stable stylesheet, while branding and content-facing style choices can still change at runtime.

## The Core Idea

Crudspa themes aren't arbitrary CSS text blobs. They are structured data.

At runtime, the system combines three layers:

| Layer | Purpose |
| --- | --- |
| Host stylesheet CSS | structural framework, module, vendor, and host styling compiled from explicit SCSS entrypoints |
| Portal `Styles` data | curated theme tokens such as colors, fonts, padding, roundness, and zoom |
| Portal `Fonts` data | named uploaded font files that theme rules can reference |

That structure keeps theming safe, readable, and consistent. Developers aren't expected to inject random CSS into the system. They edit a curated set of design tokens that the framework already understands.

## Stylesheet Versus Theme

Crudspa styling makes a hard distinction between stylesheet and theme.

A stylesheet answers this question:

* what structural SCSS stack does this host use?

A theme answers this question:

* what token values should this portal apply to that stack?

This separation is the key architectural idea.

The host stylesheet is compiled from SCSS entrypoints such as `framework-core.scss`, `content-display.scss`, `education-common.scss`, and a final host stylesheet such as `portals-provider.scss`.

The theme is generated at runtime as CSS custom properties and `@font-face` declarations for a specific portal revision.

Because those concerns are separated, Crudspa can cache host stylesheet CSS independently of portal theme changes, while still letting administrators reskin a site from structured data.

## Defaults Contract

Crudspa's theme contract centers on `defaults.scss`.

That file holds the platform's default and derived token values in one place. When the host stylesheet is compiled, the same contract can expose those values through browser-native CSS custom properties so the active portal theme can be swapped without rebuilding the entire structural stylesheet.

This works because Crudspa uses the same vocabulary at both levels:

* compile-time Sass defaults for the base framework and host stylesheets
* runtime CSS custom properties for portal-specific theme values

Some values, especially responsive breakpoints, remain compile-time Sass values because they are used in `@media` rules. Themeable visual values such as colors, fonts, spacing, and roundness can still flow through runtime tokens.

## Why This Structure Works

The theme contract stays structured and curated. Derived defaults still live in Sass. Host stylesheets remain stable and cacheable across portal revisions. Previews can scope theme tokens cleanly. Style editing stays self-contained inside the current host.

In practice, this means the platform remains highly themeable while also being easier to reason about and faster to serve.

## The `Styles` Feature

The `Styles` feature stores theme rules for a portal. Each rule has a stable key and a rule type. Current rule types include:

| Rule Type | Example | Theme Values Produced |
| --- | --- | --- |
| `Color` | `Hyperlink | Color` | one token such as `hyperlinkForegroundColor` |
| `Color Pair` | `Window | Colors` | paired tokens such as `windowBackgroundColor` and `windowForegroundColor` |
| `Font` | `Window | Title | Font` | family, size, and weight tokens |
| `Margin` | `Footer | Margin` | spacing tokens |
| `Padding` | `Heading | Padding` | spacing tokens |
| `Roundness` | `Button | Roundness` | border-radius style tokens |
| `Zoom` | `Body | Zoom` | the responsive html font-size tokens |

The rule catalog covers a wide range of framework surfaces, including:

* body, headings, labels, inputs, hyperlinks, buttons, windows, modals, tabs, filters, footers, alerts, navigation, binders, pages, and reports
* custom font slots `custom1` through `custom4`
* shared layout-related values such as padding, margin, roundness, and zoom

This is a curated token system, not a free-form CSS editor.

## The `Fonts` Feature

The `Fonts` feature stores uploaded font files and associates them with a portal.

Each font record includes:

* a human-readable name
* a file record
* a portal association

Font rules in `Styles` can reference one of those uploaded fonts. When that happens, the runtime theme generator emits both `@font-face` rules and the matching font-family token values.

That keeps font handling aligned with the rest of the theme system: structured data in the database, predictable runtime CSS on the host.

## Theme Generation Pipeline

When a client requests portal styles, Crudspa performs two jobs:

1. Resolve the current host stylesheet.
2. Load the current portal's `Styles` and `Fonts` records from the database.
3. Convert those records into Sass variable overrides.
4. Import `defaults.scss` and emit the full token set as CSS custom properties.
5. Cache the final theme result by portal revision and build.

That portal revision value is important. Style and font changes increment the portal's revision, which gives the cache a natural invalidation key. In practice, that means theme changes roll out cleanly without requiring a new application deployment.

## What The Generated Theme CSS Looks Like

A simplified generated theme sheet looks like this:

```css
@font-face {
    font-family: 'Lato';
    src: url(/api/framework/core/font-file/fetch?id=...);
}

:root {
    --body-background-color: #f4f0e8;
    --body-foreground-color: #1f1f1f;
    --button-background-color: #386180;
    --button-foreground-color: #f6f6f6;
    --window-title-font-family: 'Lato', system-ui, sans-serif;
    --window-title-font-size: 1.5em;
    --window-title-font-weight: 400;
    --button-border-radius: .5em;
    --footer-padding: .5em .5em .5em .5em;
    --html-small-font-size: .8em;
    --html-medium-font-size: .85em;
}
```

The host stylesheet already references those runtime tokens through `defaults.scss`, so the portal picks up the active theme without recompiling the whole structural stylesheet.

## Theme-Aware Application SCSS

The runtime theme doesn't stop at framework chrome. Application-specific SCSS can participate too, as long as it uses shared tokens instead of hard-coded values.

For example, a portal stylesheet can write this:

```scss
.ces-student-app {
    > .header {
        @extend .c-padding-small;
        background-color: $bodyBackgroundColor;
    }
}
```

In the compiled host stylesheet, `$bodyBackgroundColor` resolves through the runtime theme bridge. That means the application-specific SCSS still picks up the active portal theme cleanly.

This is the preferred pattern for custom styling in Crudspa:

* use shared theme tokens for colors, fonts, borders, and spacing
* use framework utility classes for structural behavior
* keep feature-specific classes focused on the feature's unique visuals

## Previewing Themes

Crudspa's preview system stays self-contained.

A preview doesn't need to fetch CSS from another running portal. Instead, the current host assembles preview CSS from:

* the current host stylesheet
* the target portal's theme tokens
* scoped preview selectors

That's an important architectural choice. It keeps previews inside the host that's already doing the editing while still reusing the real theme contract. In practice, this means a preview is exact for the current host's structural stylesheet and theme-aware for other portals that share the same design-token contract.

## Custom Font Slots And Box-Level Overrides

Crudspa still supports a second level of styling for content-heavy scenarios.

The global theme sets shared defaults, but individual content boxes can still override:

* background color and background image
* border color, border thickness, and border radius
* font size and font weight
* foreground color
* margin and padding
* box shadow and text shadow
* heading and paragraph line height
* one of the custom font slots `CustomFont1` through `CustomFont4`

Those box-level settings are intentionally separate from the main theme. The theme defines the shared visual language of a portal. Box overrides let editors shape a specific piece of content without redefining the entire application.

## Practical Guidance

* Start by theme-enabling shared values in `defaults.scss`.
* When writing portal-specific SCSS, prefer shared theme tokens over literal hex colors and hard-coded font families.
* Use `Styles` for global design tokens and `Fonts` for reusable uploaded typefaces.
* Use box-level overrides for local editorial exceptions, not for replacing the job of the theme.
* If you need a new themeable concept, add a new rule and wire it into the token contract instead of creating ad-hoc CSS injection paths.

## Tradeoffs

Crudspa's theming system is structured. Only the parts of the stylesheet that depend on the shared token contract will respond to theme changes.

That's an intentional trade. The goal isn't arbitrary CSS injection. The goal is a strong shared design system that can still be re-skinned quickly and safely.

## Next Steps

* [Styling | Stylesheets](Stylesheets.md)
* [Styling | Layouts](Layouts.md)
* [Components | Layouts](../Components/Layouts.md)
* [Components | Forms](../Components/Forms.md)
* [Documentation Index](../ReadMe.md)

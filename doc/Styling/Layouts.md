# Styling | Layouts

CRUD screens rarely fail because of one dramatic layout decision. They fail because of dozens of tiny, local decisions: a custom width here, a one-off margin there, a media query copied from another page, a toolbar that only works on desktop, a modal that looks fine until the label text gets longer.

Crudspa's layout system was built to stop that drift. Our CSS is designed specifically for CRUD+SPA work: dense forms, repeated cards, action toolbars, tabbed panes, and navigation shells that need to stay readable on large monitors and still work naturally on phones. The framework's default goal is simple: let screens reflow by composition instead of by page-specific rescue CSS.

Most developers do not need to write raw layout classes at all. They compose screens from components such as `Wrapped`, `Stacked`, `Padding`, `Margin`, `Field`, `Toolbar`, `ButtonGroup`, and `BatchSection`. Those components apply the right classes for them.

A well-composed Crudspa form may show several fields on one row at desktop widths and then wrap those same fields into a vertical stack on narrower screens, without changing the page-specific markup. That reflow behavior is the point of the layout system.

## Goals

Our layout CSS framework aims to do five things well:

* Make the common CRUD layout patterns obvious and repeatable.
* Keep forms usable on narrow screens without requiring custom breakpoint logic on each page.
* Give teams a shared vocabulary for width, spacing, alignment, and wrapping.
* Hide raw CSS details behind components in the common case.
* Leave a clean escape hatch for custom visuals when a screen really is unique.

That last point matters. The framework does not assume every screen should look the same. It aims to make the standard screens predictable and easy to compose, while still leaving room for custom visuals where they are needed.

## Default Approach

For most application work, you do not start with `<div class="c-wrap">`. You start with components. A real edit surface in a current Crudspa administrative client looks like this:

```razor
<Toolbar>
    <Left>
        <ButtonGroup>
            @if (Model.ReadOnly)
            {
                <ButtonEdit Clicked="() => Model.Edit()" />
            }
            else
            {
                <ButtonSave Clicked="() => Model.Save()" />
                <ButtonCancel Clicked="HandleCancelClicked" />
            }
        </ButtonGroup>
    </Left>
</Toolbar>
<Waiter Model="Model">
    @if (Model.Entity is not null)
    {
        <Padding Size="Padding.Sizes.Small"
                 Direction="Padding.Directions.Vertical">
            <Wrapped Alignment="Wrapped.Alignments.Top">
                <Field Label="Title"
                       Size="Field.Sizes.Wide">
                    <TextBox ReadOnly="Model.ReadOnly"
                             MaxLength="150"
                             @bind-Value="Model.Entity.Title" />
                </Field>
                <Field Label="Status"
                       Size="Field.Sizes.Unspecified">
                    <Radio ReadOnly="Model.ReadOnly"
                           LookupValues="Model.ContentStatusNames"
                           @bind-Value="Model.Entity.StatusId" />
                </Field>
                <Field Label="Author"
                       Size="Field.Sizes.Wide">
                    <TextBox ReadOnly="Model.ReadOnly"
                             MaxLength="150"
                             @bind-Value="Model.Entity.Author" />
                </Field>
            </Wrapped>
        </Padding>
    }
</Waiter>
```

There is no page-specific media query here. There is no custom width math. The screen works because the layout system already knows how fields should size, wrap, pad, and stack.

That pattern is common across Crudspa's administrative UI. Most of those Razor files are primarily component composition rather than hand-authored layout markup.

## Flexbox Primer

Crudspa's layout system depends heavily on flexbox, but most developers only need a small subset of flexbox concepts to be productive.

| Concept | What It Means | How Crudspa Uses It |
| --- | --- | --- |
| Main axis | The direction items flow in a container | `Wrapped` and `NoWrap` use a horizontal row; `Stacked` uses a vertical column |
| Cross axis | The direction perpendicular to the main axis | Alignment options such as `Top`, `Center`, `Bottom`, and `Right` map to cross-axis or content alignment choices |
| Wrapping | Whether items move to a new line when space runs out | `Wrapped` defaults to wrapping, which is why form rows collapse gracefully on smaller screens |
| Basis, grow, shrink | How an item claims space and how willing it is to stretch or compress | `Field` sizes provide the common default behavior; advanced editors can expose basis, grow, and shrink directly |
| Min width | The smallest useful width before a control becomes cramped | Named sizes such as `Tiny`, `Medium`, `Wide`, and `Max` keep controls readable and encourage wrapping before collapse |
| Justify and align | How extra space is distributed inside the container | Shared components encode the common alignments so pages do not need to repeat raw CSS |

If you come from XAML, WPF, or Silverlight, Crudspa's `Star` language will feel familiar. Star sizing means "take the remaining space." In CSS terms, the framework's star helper maps to flexible growth with a safe minimum width.

## Component Vocabulary

The layout components are intentionally thin wrappers over the CSS framework. They mostly translate C# intent into stable class names.

| Component Or Option | Typical Output | Role |
| --- | --- | --- |
| `Wrapped` | `c-wrap` or `c-nowrap` | Horizontal flex row, usually with wrapping enabled |
| `Stacked` | `c-stack` | Vertical stack of children |
| `Padding` | `c-padding-small`, `c-padding-medium-top`, and similar | Inner spacing |
| `Margin` | `c-margin-small`, `c-margin-large-hor`, and similar | Outer spacing |
| `Field Size="Wide"` | `c-field wide` | Shared width vocabulary for labeled controls |
| `Field Size="Star"` | `c-field star` | Flexible field that takes remaining row space |
| `Border` | `c-border`, `c-border-top`, `c-border-hor`, and similar | Semantic borders without custom CSS |
| `ButtonGroup` | `c-button-group` | No-wrap action cluster with joined button edges |
| `Toolbar` | `c-toolbar` | Standard left/right action strip |

The abstraction stays small enough to learn quickly while still giving teams a consistent layout vocabulary.

## Why It Works On Phones

Crudspa gets responsive behavior from a few simple defaults working together:

* Rows wrap by default instead of fighting to stay on one line.
* Controls have named widths and minimum widths, so they wrap before they become unusably narrow.
* Spacing values change across screen sizes.
* The root font size also changes across screen sizes, so text and spacing scale together.
* Navigation shells, tab sets, windows, and modals already contain mobile-aware behavior.

The result is that many screens become responsive without any page-specific breakpoint code. Developers can simply lay out forms in terms of `Field`, `Wrapped`, `Padding`, and `Toolbar`, and the framework does the hard work.

## Raw CSS Still Has A Place

Component-first does not mean "never write CSS." It means "use custom CSS for custom visuals, not for basic screen structure."

A good example is a feature-specific surface that still leans on the shared layout vocabulary:

```scss
.ces-avatar-options {
    @extend .c-wrap;
    justify-content: center;
    max-height: 24em;
    overflow-y: auto;
    padding: .5em;
    background-color: $inputBackgroundColor;
    color: $inputForegroundColor;
    border: $inputBorderWidth $inputBorderStyle $inputBorderColor;
    border-radius: $inputBorderRadius;
}
```

That pattern comes up often in Crudspa applications. The custom class is feature-specific, but it still builds on framework layout primitives and theme variables. In practice, this keeps custom screens aligned with the rest of the application instead of turning into their own parallel design system.

## Practical Guidance

* Start with `Field`, `Wrapped`, `Stacked`, `Padding`, and `Toolbar` before writing layout CSS.
* Use `Wrapped` for rows that may need to collapse. Use `NoWrap` only for tight action groups and similar cases.
* Use `Padding` for inside space and `Margin` for outside space.
* Prefer named sizes over pixel math. `Tiny`, `Medium`, `Wide`, and `Star` are easier to read and easier to standardize.
* When you do need custom CSS, keep it focused on feature-specific visuals and keep it anchored to shared framework variables and utility classes.

## Tradeoffs

Crudspa's layout system is opinionated. It asks teams to work within a shared vocabulary for layout, spacing, and sizing. In return, screens are easier to review and keep consistent.

Developers do need to learn that vocabulary, even if they do not write much raw CSS. The set of concepts is small, but it shows up throughout the framework.

## Next Steps

* [Components | Layouts](../Components/Layouts.md)
* [Components | Forms](../Components/Forms.md)
* [Styling | Stylesheets](Stylesheets.md)
* [Styling | Theming](Theming.md)
* [Documentation Index](../ReadMe.md)

# Components | Textboxes

Text input starts simple, then quickly adds debounce, masking, numeric formatting, and rich text safety requirements.

Crudspa provides a focused textbox family so each text scenario has one clear default.

Some members of this family are native Crudspa inputs. Others are focused Radzen wrappers that keep a smaller, Crudspa-shaped surface.

## Component Catalog

 Component | Use It For | Key Features | Implementation
 --- | --- | --- | ---
 `TextBox` | standard single-line text | `oninput` binding, max length, placeholder | native html `input`
 `MultilineTextBox` | multi-line plain text | height presets, optional code mode | native html `textarea`
 `SearchTextBox` | debounced search/filter text | timer-based debounce, configurable delay | Radzen `RadzenTextBox` wrapper
 `MaskedTextBox` | pattern-constrained text | mask + character pattern support | Radzen `RadzenMask` wrapper
 `NumericTextBox<T>` | numeric entry | typed value + format string | Radzen `RadzenNumeric` wrapper
 `HtmlEditor` | rich-text edit/display with sanitization | toolbar, token insertion, paste cleanup | Radzen `RadzenHtmlEditor` plus Crudspa safety tooling
 `UserHtml` | safe trusted-html display with link interception | JS link interception + event forwarding | Crudspa display component

## Default Approach

Use `TextBox` for short text and `MultilineTextBox` for longer plain text:

```razor
<Field Label="Name"
       Size="Field.Sizes.Wide">
    <TextBox ReadOnly="Model.ReadOnly"
             MaxLength="100"
             @bind-Value="Model.Entity.Name" />
</Field>

<Field Label="Street Address"
       Size="Field.Sizes.Wide">
    <MultilineTextBox ReadOnly="ReadOnly"
                      Height="MultilineTextBox.Heights.Short"
                      @bind-Value="UsaPostal.StreetAddress" />
</Field>
```

## Option Reference

### `TextBox`

 Parameter | Purpose
 --- | ---
 `Value` / `ValueChanged` | text value binding
 `Placeholder` | prompt text
 `MaxLength` | max character count
 `ReadOnly` | disables editing

### `MultilineTextBox`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Value` / `ValueChanged` | text value | supports `@bind-Value`
 `Height` | height class | `Mini`, `Short`, `Average`, `Tall`, `TallX2`, `TallX3`, `TallX4`
 `Code` | code-style mode | disables spellcheck and applies code styling
 `Spellcheck` | browser spellcheck toggle | default `true`
 `ReadOnly`, `Placeholder` | standard text options | optional

### `SearchTextBox`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Value` / `ValueChanged` | search text | supports `@bind-Value`
 `Debounce` | delay in milliseconds | default `250`
 `Placeholder`, `MaxLength`, `ReadOnly` | standard text options | optional

### `MaskedTextBox`

 Parameter | Purpose
 --- | ---
 `Mask` | visual/input mask pattern
 `CharacterPattern` | regex-like allowed characters
 `Placeholder` | placeholder pattern text
 `Value` / `ValueChanged`, `ReadOnly` | binding and read mode

### `NumericTextBox<T>`

 Parameter | Purpose
 --- | ---
 `Value` / `ValueChanged` | typed numeric value binding
 `Format` | numeric format string (default `"D"`)
 `ReadOnly` | disables editing

### `HtmlEditor`

 Parameter | Purpose | Notes
 --- | --- | ---
 `Value` / `ValueChanged` | html content | sanitized on paste
 `ReadOnly` | display mode vs editor mode | read-only mode renders via `UserHtml`
 `EditorHeight` | editor height style | default `"24em"`
 `DisplayHeight` | read-only max-height with scroll | optional
 `AllowImages` | allow pasted image tags | default `false`
 `Tokens` | insertable token list | optional token modal/tool

### `UserHtml`

 Parameter | Purpose
 --- | ---
 `Html` | rendered html string
 `CssClass` | additional css class
 `ChildContent` | optional extra markup after html

## Radzen-Backed Wrappers

`SearchTextBox`, `MaskedTextBox`, `NumericTextBox<T>`, and editable `HtmlEditor` all sit on top of Radzen controls. Crudspa trims their public surface to the behavior we use most often in forms, filters, and editors. If you need a lower-level Radzen option that Crudspa doesn't expose, you can use the Radzen component directly because the library is bundled with the framework.

## Rich Text Safety Model

`HtmlEditor` sanitizes paste input by:

* removing dangerous tags (`script`, `style`, `iframe`, `form`, and others)
* removing risky attributes (`on*`, `id`, `lang`, `dir`, `data-id`)
* blocking `javascript:` URLs in `href` and `src`
* optionally removing images when `AllowImages=false`

This default protects CRUD editors from common injected markup issues while preserving useful formatting.

## Framework Integration

* `SearchTextBox` is ideal in list filter rows and works with model filter handlers.
* `HtmlEditor` uses a modal token picker (`InsertTokenModel`) and standard modal/button components.
* `UserHtml` integrates with `ILinkClickService` through JS bridge interception.

## Practical Guidance

* Use `SearchTextBox` for filter/search only, not ordinary form fields.
* Keep masks strict for structured values (phone, postal codes, account codes).
* Keep numeric formats explicit when precision or scale needs to stay obvious.
* Use `UserHtml` directly only when html is already trusted/sanitized.

## Common Questions

### Should I use `SearchTextBox` outside filter UIs?

Usually no. Debounce behavior is optimized for search patterns.

### Is `HtmlEditor` safe for arbitrary pasted content?

It applies strong sanitization by default. Keep `AllowImages=false` unless image tags are explicitly needed.

### When should I use `UserHtml` directly?

When your model already has trusted html and you still want consistent rendering and link interception.

## Tradeoffs

Textbox wrappers expose a smaller surface than raw third-party controls, and several of them are intentionally thin Radzen wrappers. You lose some low-level control, but gain predictable behavior and easier maintenance across modules.

Rich text also has a natural edit/read split. `HtmlEditor` owns authoring behavior, while `UserHtml` owns safe, consistent rendering when the screen is back in read mode.

## Next Steps

* [Components | Dropdowns](Dropdowns.md)
* [Components | Pickers](Pickers.md)
* [Types | Text](../Types/Text.md)
* [Documentation Index](../ReadMe.md)

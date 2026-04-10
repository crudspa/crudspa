# Types | Text

Text appears everywhere in CRUD systems, but not all text is the same. Short labels, search strings, long guidance content, and rich HTML each have different risk and validation needs.

Crudspa keeps text reliable by pairing each text scenario with a specific control, parameter mapping pattern, and validation strategy.

## Default Approach

Use these defaults:

* bounded short text: SQL `nvarchar(n)` + `TextBox` + explicit max length.
* longer plain text: SQL `nvarchar(max)` + `MultilineTextBox`.
* search/filter text: `SearchTextBox` + parameterized `LIKE` queries.
* rich text: `HtmlEditor` on client, sanitize again on server before save.

## Nullability Policy

Most text DTO properties are nullable by design:

* `null` is often a real domain state (optional note, empty guidance, absent filter).
* null values are omitted during JSON serialization.
* required/format/length rules are enforced in validation and server logic.

Crudspa serialization behavior keeps null text values out of payloads:

```csharp
var options = new JsonSerializerOptions
{
    DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
    WriteIndented = pretty,
};
```

## Text Shape Reference

| Text Shape | SQL Pattern | Component | Typical Validation |
| --- | --- | --- | --- |
| Short required fields | `nvarchar(75)`, `nvarchar(150)` | `TextBox` | required + max length |
| Long plain text | `nvarchar(max)` | `MultilineTextBox` | optional length and content checks |
| Search/filter input | `nvarchar(50)` parameter | `SearchTextBox` | bounded length |
| Rich HTML | `nvarchar(max)` | `HtmlEditor` + `UserHtml` | sanitize client and server |

## SQL And Sproxy Mapping

Use bounded SQL text columns where possible:

```sql
create table [Education].[Unit] (
    [Title] nvarchar(75) not null,
    [GuideText] nvarchar(max) null
);
```

Use explicit parameter lengths for bounded text:

```csharp
command.AddParameter("@Title", 75, unit.Title);
command.AddParameter("@GuideText", unit.GuideText);
```

For search text, keep a bounded parameter:

```csharp
command.AddParameter("@SearchText", 50, search.Text);
```

and keep SQL filtering parameterized:

```sql
and (@SearchText is null
    or post.Body like '%' + @SearchText + '%'
)
```

## UI Patterns

Short text:

```razor
<TextBox ReadOnly="Model.ReadOnly"
         MaxLength="75"
         @bind-Value="Model.Entity.Title" />
```

Long plain text:

```razor
<MultilineTextBox ReadOnly="Model.ReadOnly"
                  Height="MultilineTextBox.Heights.Tall"
                  @bind-Value="Model.Entity.GuideText" />
```

Search with debounce:

```razor
<SearchTextBox @bind-Value="Model.Search.Text" />
```

Rich text:

```razor
<HtmlEditor ReadOnly="Model.ReadOnly"
            @bind-Value="Model.Entity.Body" />
```

For full textbox and editor options, see [Components | Textboxes](../Components/Textboxes.md).

In UI terms, short labels and codes usually want `TextBox`, longer notes want `MultilineTextBox`, search inputs want `SearchTextBox`, and authored body content wants the `HtmlEditor` and `UserHtml` pair. The important distinction isn't only length. Each text shape carries different behavior, validation, and rendering needs.

## Rich Text Safety

Client-side `HtmlEditor` strips dangerous tags and attributes on paste, including script/event patterns:

```csharp
if (name.StartsWith("on"))
    return true;

if (name is "href" or "src"
    && attribute.Value.TrimStart().StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
    return true;
```

Server services sanitize again before persistence:

```csharp
post.Body = htmlSanitizer.Sanitize(post.Body);
```

The double-pass model is intentional. Client cleanup improves UX, server sanitization is the final trust boundary.

## Validation Integration

Keep text rules in contracts:

```csharp
if (Email.HasNothing())
    errors.AddError("Email Address is required.", nameof(Email));
else if (Email!.Length > 75)
    errors.AddError("Email Address cannot be longer than 75 characters.", nameof(Email));
else if (!Email.IsEmailAddress())
    errors.AddError("Email Address must be properly formatted.", nameof(Email));
```

This keeps behavior consistent across UI flows, API usage, and batch operations.

## Tradeoffs

Using bounded lengths and explicit validation adds a little upfront work, but it prevents common production drift between SQL, UI, and service behavior.

For rich text, stricter sanitization can remove some pasted formatting. That's usually the safer production default.

## Next Steps

* [Types | File](File.md)
* [Components | Textboxes](../Components/Textboxes.md)
* [Documentation Index](../ReadMe.md)

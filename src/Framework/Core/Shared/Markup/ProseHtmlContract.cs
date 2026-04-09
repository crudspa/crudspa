namespace Crudspa.Framework.Core.Shared.Markup;

public static class ProseHtmlContract
{
    public const String EmptyParagraphHtml = "<p><br></p>";

    public static IReadOnlyList<String> CanonicalStyleOrder { get; } =
    [
        "color",
        "background-color",
        "font-style",
        "font-weight",
        "text-decoration",
        "text-decoration-line",
        "text-align",
    ];

    public static HashSet<String> AllowedTags { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "a",
        "blockquote",
        "br",
        "em",
        "h1",
        "h2",
        "h3",
        "h4",
        "li",
        "ol",
        "p",
        "span",
        "strong",
        "u",
        "ul",
    };

    public static HashSet<String> AllowedStyleProperties { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "background-color",
        "color",
        "font-style",
        "font-weight",
        "text-align",
        "text-decoration",
        "text-decoration-line",
    };

    public static HashSet<String> SanitizerAllowedAttributes { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "href",
        "rel",
        "style",
        "target",
    };

    public static HashSet<String> SanitizerAllowedSchemes { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "http",
        "https",
        "mailto",
    };

    public static HashSet<String> BlockTags { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "blockquote",
        "div",
        "h1",
        "h2",
        "h3",
        "h4",
        "h5",
        "h6",
        "li",
        "ol",
        "p",
        "section",
        "table",
        "tbody",
        "td",
        "tfoot",
        "th",
        "thead",
        "tr",
        "ul",
    };

    public static HashSet<String> ParagraphLikeTags { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "blockquote",
        "h1",
        "h2",
        "h3",
        "h4",
        "p",
    };

    public static HashSet<String> LegacyWrapperTags { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "address",
        "article",
        "aside",
        "caption",
        "center",
        "code",
        "col",
        "colgroup",
        "dd",
        "div",
        "dl",
        "dt",
        "figure",
        "figcaption",
        "footer",
        "header",
        "main",
        "pre",
        "section",
        "table",
        "tbody",
        "td",
        "tfoot",
        "th",
        "thead",
        "tr",
    };

    public static HashSet<String> RemoveWithContentTags { get; } = new(StringComparer.OrdinalIgnoreCase)
    {
        "audio",
        "button",
        "embed",
        "form",
        "iframe",
        "input",
        "object",
        "script",
        "select",
        "style",
        "textarea",
        "video",
    };
}
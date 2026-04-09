using System.Text.RegularExpressions;
using AngleSharp.Dom;
using AngleSharp.Html.Parser;

namespace Crudspa.Framework.Core.Shared.Markup;

public static class ProseHtmlNormalizer
{
    private static readonly Regex HexColorRegex = new("^#(?:[0-9a-f]{3}|[0-9a-f]{4}|[0-9a-f]{6}|[0-9a-f]{8})$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex FunctionalColorRegex = new("^(?:rgb|rgba|hsl|hsla)\\([^)]*\\)$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex NamedColorRegex = new("^[a-z][a-z0-9-]*$", RegexOptions.Compiled | RegexOptions.IgnoreCase);
    private static readonly Regex UnsafeProtocolWhitespaceRegex = new("[\\u0000-\\u0020]+", RegexOptions.Compiled);

    public static String? Normalize(String? html, Boolean allowImages = false) => NormalizeForStorage(html, allowImages);

    public static String? NormalizeForPaste(String? html, Boolean allowImages = false) => NormalizeForStorage(html, allowImages);

    public static String? NormalizeForStorage(String? html, Boolean allowImages = false)
    {
        if (html.HasNothing())
            return html;

        var body = ParseBody(html!);
        NormalizeContainer(body, new(allowImages));

        var normalized = body.InnerHtml.Trim();

        return normalized.HasSomething() ? normalized : null;
    }

    public static Boolean IsEmpty(String? html)
    {
        var normalized = NormalizeForStorage(html);

        return normalized.HasNothing()
            || normalized.IsBasically(ProseHtmlContract.EmptyParagraphHtml);
    }

    private static IElement ParseBody(String html)
    {
        var parser = new HtmlParser();
        var document = parser.ParseDocument("<!doctype html><html><body></body></html>");
        var body = document.Body!;

        foreach (var node in parser.ParseFragment(html, body).ToArray())
            body.AppendChild(node);

        return body;
    }

    private static void NormalizeContainer(IElement container, HtmlOptions options)
    {
        foreach (var child in container.ChildNodes.ToArray())
            NormalizeNode(child, options);

        var name = container.LocalName.ToLowerInvariant();

        if (name is "ul" or "ol")
        {
            CanonicalizeList(container);
            return;
        }

        if (name == "li")
        {
            CanonicalizeListItem(container);
            return;
        }

        if (name is "body" or "blockquote")
            CanonicalizeBlockContainer(container);
    }

    private static void NormalizeNode(INode node, HtmlOptions options)
    {
        switch (node)
        {
            case IComment:
                RemoveNode(node);
                return;
            case IText text:
                NormalizeText(text);
                return;
            case IElement element:
                NormalizeElement(element, options);
                return;
            default:
                RemoveNode(node);
                return;
        }
    }

    private static void NormalizeText(IText text)
    {
        text.Data = text.Data.Replace('\u00A0', ' ');
    }

    private static void NormalizeElement(IElement element, HtmlOptions options)
    {
        var name = element.LocalName.ToLowerInvariant();

        if ((!options.AllowImages && name == "img")
            || ProseHtmlContract.RemoveWithContentTags.Contains(name))
        {
            element.Remove();
            return;
        }

        element = NormalizeLegacyElement(element);
        name = element.LocalName.ToLowerInvariant();

        SanitizeAttributes(element, options);

        foreach (var child in element.ChildNodes.ToArray())
            NormalizeNode(child, options);

        name = element.LocalName.ToLowerInvariant();

        if (name is "html" or "body")
        {
            Unwrap(element);
            return;
        }

        if (name == "span")
        {
            element = PromoteSimpleSpanFormatting(element);
            name = element.LocalName.ToLowerInvariant();
        }

        if (!IsAllowedElement(name, options))
        {
            if (IsBlockLike(name))
            {
                CanonicalizeLegacyBlock(element);
                return;
            }

            Unwrap(element);
            return;
        }

        if (name == "a" && !element.HasAttribute("href"))
        {
            Unwrap(element);
            return;
        }

        if (name == "span" && !element.Attributes.Any())
        {
            Unwrap(element);
            return;
        }

        if (name is "ul" or "ol")
        {
            CanonicalizeList(element);
            if (!element.Children.Any())
                element.Remove();

            return;
        }

        if (name == "li")
        {
            CanonicalizeListItem(element);

            if (!HasVisibleContent(element) && !element.Children.Any(x => x.LocalName is "ul" or "ol"))
                element.Remove();

            return;
        }

        if (name == "blockquote")
        {
            CanonicalizeBlockContainer(element);

            if (!element.Children.Any() && !HasVisibleContent(element))
                element.Remove();

            return;
        }

        if (ProseHtmlContract.ParagraphLikeTags.Contains(name))
        {
            if (HasDirectBlockChildren(element))
            {
                Unwrap(element);
                return;
            }

            CanonicalizeParagraphLike(element);
        }
    }

    private static IElement NormalizeLegacyElement(IElement element)
    {
        if (element.LocalName.IsBasically("font"))
        {
            if (element.GetAttribute("color") is { } color && SanitizeColor(color) is { } sanitizedColor)
                ApplyStyle(element, "color", sanitizedColor);

            element.RemoveAttribute("color");
            element.RemoveAttribute("face");
            element.RemoveAttribute("size");
            return Rename(element, "span");
        }

        if (element.LocalName.IsBasically("center"))
        {
            ApplyStyle(element, "text-align", "center");
            return Rename(element, "div");
        }

        if (element.LocalName.IsBasically("b"))
            return Rename(element, "strong");

        if (element.LocalName.IsBasically("i"))
            return Rename(element, "em");

        if (element.LocalName.IsBasically("h5") || element.LocalName.IsBasically("h6"))
            return Rename(element, "h4");

        if (element.GetAttribute("align") is { } align && SanitizeTextAlign(align) is { } sanitizedAlign)
            ApplyStyle(element, "text-align", sanitizedAlign);

        element.RemoveAttribute("align");

        return element;
    }

    private static void SanitizeAttributes(IElement element, HtmlOptions options)
    {
        foreach (var attribute in element.Attributes.ToArray())
        {
            var name = attribute.Name.ToLowerInvariant();

            if (name.StartsWith("on", StringComparison.Ordinal)
                || name.StartsWith("data-", StringComparison.Ordinal)
                || name is "align" or "class" or "contenteditable" or "dir" or "id" or "lang")
            {
                element.RemoveAttribute(attribute.Name);
                continue;
            }

            if (!IsAllowedAttribute(element, name, options))
            {
                element.RemoveAttribute(attribute.Name);
                continue;
            }

            if (name == "style")
            {
                var sanitizedStyle = SanitizeStyle(attribute.Value, element);

                if (sanitizedStyle.HasNothing())
                    element.RemoveAttribute(attribute.Name);
                else
                    element.SetAttribute(attribute.Name, sanitizedStyle);

                continue;
            }

            if (name == "href")
            {
                var sanitizedHref = SanitizeUrl(attribute.Value);

                if (sanitizedHref.HasNothing())
                    element.RemoveAttribute(attribute.Name);
                else
                    element.SetAttribute(attribute.Name, sanitizedHref);

                continue;
            }

            if (name == "src")
            {
                var sanitizedSrc = SanitizeUrl(attribute.Value);

                if (sanitizedSrc.HasNothing())
                    element.RemoveAttribute(attribute.Name);
                else
                    element.SetAttribute(attribute.Name, sanitizedSrc);
            }
        }
    }

    private static Boolean IsAllowedAttribute(IElement element, String name, HtmlOptions options)
    {
        if (name == "style")
            return element.LocalName is not "br";

        if (element.LocalName.IsBasically("a"))
            return name == "href";

        if (options.AllowImages && element.LocalName.IsBasically("img"))
            return name is "alt" or "src" or "title";

        return false;
    }

    private static String? SanitizeStyle(String? style, IElement element)
    {
        if (style.HasNothing())
            return null;

        var rules = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        foreach (var part in style!.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
        {
            var pair = part.Split(':', 2, StringSplitOptions.TrimEntries);

            if (pair.Length != 2)
                continue;

            var name = pair[0].ToLowerInvariant();
            var value = pair[1];

            if (name == "background" && SanitizeColor(value) is { } backgroundColor)
            {
                name = "background-color";
                value = backgroundColor;
            }

            if (!ProseHtmlContract.AllowedStyleProperties.Contains(name))
                continue;

            if (name == "text-align" && !IsAlignableElement(element))
                continue;

            var sanitizedValue = SanitizeStyleValue(name, value);

            if (sanitizedValue.HasSomething())
                rules[name] = sanitizedValue;
        }

        var ordered = ProseHtmlContract.CanonicalStyleOrder
            .Where(rules.ContainsKey)
            .Select(name =>
            {
                var actualName = name is "text-decoration" && rules.ContainsKey("text-decoration-line")
                    ? "text-decoration-line"
                    : name;

                return actualName;
            })
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .Select(name => $"{name}: {rules[name]}")
            .ToList();

        return ordered.Count == 0 ? null : String.Join("; ", ordered);
    }

    private static String? SanitizeStyleValue(String name, String value)
    {
        return name switch
        {
            "background-color" => SanitizeColor(value),
            "color" => SanitizeColor(value),
            "font-style" => SanitizeFontStyle(value),
            "font-weight" => SanitizeFontWeight(value),
            "text-align" => SanitizeTextAlign(value),
            "text-decoration" => SanitizeTextDecoration(value),
            "text-decoration-line" => SanitizeTextDecoration(value),
            _ => null,
        };
    }

    private static String? SanitizeColor(String? value)
    {
        if (value.HasNothing())
            return null;

        value = value!.Trim();

        if (!HexColorRegex.IsMatch(value)
            && !FunctionalColorRegex.IsMatch(value)
            && !NamedColorRegex.IsMatch(value))
            return null;

        return value;
    }

    private static String? SanitizeFontStyle(String? value)
    {
        if (value.HasNothing())
            return null;

        value = value!.Trim().ToLowerInvariant();

        return value.Contains("italic", StringComparison.Ordinal)
            || value.Contains("oblique", StringComparison.Ordinal)
            ? "italic"
            : null;
    }

    private static String? SanitizeFontWeight(String? value)
    {
        if (value.HasNothing())
            return null;

        value = value!.Trim().ToLowerInvariant();

        if (value is "bold" or "bolder")
            return "bold";

        if (Int32.TryParse(value, out var numericWeight) && numericWeight >= 600)
            return "bold";

        return null;
    }

    private static String? SanitizeTextAlign(String? value)
    {
        if (value.HasNothing())
            return null;

        value = value!.Trim().ToLowerInvariant();

        if (value.Contains("center", StringComparison.Ordinal))
            return "center";

        if (value.Contains("right", StringComparison.Ordinal) || value.Contains("end", StringComparison.Ordinal))
            return "right";

        if (value.Contains("left", StringComparison.Ordinal) || value.Contains("start", StringComparison.Ordinal))
            return "left";

        return null;
    }

    private static String? SanitizeTextDecoration(String? value)
    {
        if (value.HasNothing())
            return null;

        value = value!.Trim().ToLowerInvariant();

        return value.Contains("underline", StringComparison.Ordinal)
            ? "underline"
            : null;
    }

    private static String? SanitizeUrl(String? value)
    {
        if (value.HasNothing())
            return null;

        var trimmed = value!.Trim();
        var compact = UnsafeProtocolWhitespaceRegex.Replace(trimmed, String.Empty);

        if (compact.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase)
            || compact.StartsWith("data:", StringComparison.OrdinalIgnoreCase)
            || compact.StartsWith("vbscript:", StringComparison.OrdinalIgnoreCase))
            return null;

        return trimmed.RemoveFirst("about:///");
    }

    private static IElement PromoteSimpleSpanFormatting(IElement element)
    {
        if (!element.LocalName.IsBasically("span")
            || element.Attributes.Length != 1
            || !element.HasAttribute("style"))
            return element;

        var style = element.GetAttribute("style");

        return style switch
        {
            "font-style: italic" => RenameWithoutAttributes(element, "em"),
            "font-weight: bold" => RenameWithoutAttributes(element, "strong"),
            "text-decoration: underline" => RenameWithoutAttributes(element, "u"),
            "text-decoration-line: underline" => RenameWithoutAttributes(element, "u"),
            _ => element,
        };
    }

    private static void CanonicalizeLegacyBlock(IElement element)
    {
        if (HasDirectBlockChildren(element))
        {
            Unwrap(element);
            return;
        }

        element = Rename(element, "p");
        CanonicalizeParagraphLike(element);
    }

    private static void CanonicalizeParagraphLike(IElement element)
    {
        TrimEdges(element);

        if (HasVisibleContent(element))
            return;

        if (!element.LocalName.IsBasically("p"))
            element = Rename(element, "p");

        ClearChildren(element);
        element.AppendChild(element.Owner!.CreateElement("br"));
    }

    private static void CanonicalizeBlockContainer(IElement container)
    {
        var rebuilt = new List<INode>();
        var inlineBuffer = new List<INode>();

        foreach (var child in container.ChildNodes.ToArray())
        {
            if (child is IText text && text.Data.HasNothing() && inlineBuffer.Count == 0)
                continue;

            if (IsBlockNode(child))
            {
                FlushInlineBuffer(container, inlineBuffer, rebuilt);
                rebuilt.Add(child);
                continue;
            }

            inlineBuffer.Add(child);
        }

        FlushInlineBuffer(container, inlineBuffer, rebuilt);
        ReplaceChildren(container, rebuilt);
    }

    private static void CanonicalizeList(IElement list)
    {
        var rebuilt = new List<INode>();
        var inlineBuffer = new List<INode>();

        foreach (var child in list.ChildNodes.ToArray())
        {
            if (child is IText text && text.Data.HasNothing() && inlineBuffer.Count == 0)
                continue;

            if (child is IElement element && element.LocalName.IsBasically("li"))
            {
                FlushListBuffer(list, inlineBuffer, rebuilt);
                rebuilt.Add(child);
                continue;
            }

            if (child is IElement nestedList && nestedList.LocalName is "ul" or "ol")
            {
                if (rebuilt.LastOrDefault() is IElement lastItem && lastItem.LocalName.IsBasically("li"))
                    lastItem.AppendChild(nestedList);
                else
                    rebuilt.Add(CreateListItem(list, [nestedList]));

                continue;
            }

            inlineBuffer.Add(child);
        }

        FlushListBuffer(list, inlineBuffer, rebuilt);
        ReplaceChildren(list, rebuilt.Where(x => x is not IElement element || element.Children.Any() || HasVisibleContent(element)).ToList());
    }

    private static void CanonicalizeListItem(IElement item)
    {
        TrimEdges(item);

        if (item.Children.Length == 1
            && item.FirstElementChild?.LocalName.IsBasically("p") == true
            && item.FirstElementChild.Attributes.Length == 0)
        {
            Unwrap(item.FirstElementChild);
            TrimEdges(item);
        }
    }

    private static void FlushInlineBuffer(IElement container, List<INode> inlineBuffer, List<INode> rebuilt)
    {
        TrimInlineBuffer(inlineBuffer);

        if (inlineBuffer.Count == 0)
            return;

        var paragraph = container.Owner!.CreateElement("p");

        foreach (var node in inlineBuffer.ToArray())
            paragraph.AppendChild(node);

        CanonicalizeParagraphLike(paragraph);
        rebuilt.Add(paragraph);
        inlineBuffer.Clear();
    }

    private static void FlushListBuffer(IElement container, List<INode> inlineBuffer, List<INode> rebuilt)
    {
        TrimInlineBuffer(inlineBuffer);

        if (inlineBuffer.Count == 0)
            return;

        rebuilt.Add(CreateListItem(container, inlineBuffer.ToArray()));
        inlineBuffer.Clear();
    }

    private static IElement CreateListItem(IElement container, IEnumerable<INode> nodes)
    {
        var item = container.Owner!.CreateElement("li");

        foreach (var node in nodes.ToArray())
            item.AppendChild(node);

        CanonicalizeListItem(item);

        return item;
    }

    private static void TrimInlineBuffer(List<INode> nodes)
    {
        while (nodes.FirstOrDefault() is IText first && first.Data.HasNothing())
            nodes.RemoveAt(0);

        while (nodes.LastOrDefault() is IText last && last.Data.HasNothing())
            nodes.RemoveAt(nodes.Count - 1);
    }

    private static void TrimEdges(IElement element)
    {
        while (element.FirstChild is IText first && first.Data.HasNothing())
            first.Remove();

        while (element.LastChild is IText last && last.Data.HasNothing())
            last.Remove();
    }

    private static void ReplaceChildren(IElement element, IList<INode> nodes)
    {
        ClearChildren(element);

        foreach (var node in nodes)
            element.AppendChild(node);
    }

    private static void ClearChildren(IElement element)
    {
        while (element.FirstChild is not null)
            RemoveNode(element.FirstChild);
    }

    private static Boolean HasVisibleContent(IElement element)
    {
        return element.ChildNodes.Any(IsVisibleNode);
    }

    private static Boolean IsVisibleNode(INode node)
    {
        return node switch
        {
            IText text => text.Data.HasSomething(),
            IElement element => element.LocalName != "br" && (element.LocalName == "img" || element.TextContent.Replace('\u00A0', ' ').HasSomething()),
            _ => false,
        };
    }

    private static Boolean IsAllowedElement(String name, HtmlOptions options)
    {
        return ProseHtmlContract.AllowedTags.Contains(name)
            || options.AllowImages && name == "img";
    }

    private static Boolean IsAlignableElement(IElement element)
    {
        return element.LocalName is "blockquote" or "h1" or "h2" or "h3" or "h4" or "li" or "ol" or "p" or "ul"
            || ProseHtmlContract.LegacyWrapperTags.Contains(element.LocalName);
    }

    private static Boolean HasDirectBlockChildren(IElement element)
    {
        return element.Children.Any(child => IsBlockLike(child.LocalName));
    }

    private static Boolean IsBlockNode(INode node)
    {
        return node is IElement element && IsBlockLike(element.LocalName);
    }

    private static Boolean IsBlockLike(String name)
    {
        return ProseHtmlContract.BlockTags.Contains(name)
            || ProseHtmlContract.LegacyWrapperTags.Contains(name);
    }

    private static IElement RenameWithoutAttributes(IElement element, String tagName)
    {
        element.RemoveAttribute("style");
        return Rename(element, tagName);
    }

    private static IElement Rename(IElement element, String tagName)
    {
        if (element.LocalName.IsBasically(tagName))
            return element;

        var replacement = element.Owner!.CreateElement(tagName);

        foreach (var attribute in element.Attributes.ToArray())
            replacement.SetAttribute(attribute.Name, attribute.Value);

        foreach (var child in element.ChildNodes.ToArray())
            replacement.AppendChild(child);

        var parent = element.Parent;

        if (parent is null)
            return replacement;

        parent.InsertBefore(replacement, element);
        element.Remove();

        return replacement;
    }

    private static void Unwrap(IElement element)
    {
        var parent = element.Parent;

        if (parent is null)
            return;

        foreach (var child in element.ChildNodes.ToArray())
            parent.InsertBefore(child, element);

        element.Remove();
    }

    private static void ApplyStyle(IElement element, String name, String value)
    {
        var existing = element.GetAttribute("style");
        var rules = new Dictionary<String, String>(StringComparer.OrdinalIgnoreCase);

        if (existing.HasSomething())
        {
            foreach (var part in existing!.Split(';', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            {
                var pair = part.Split(':', 2, StringSplitOptions.TrimEntries);

                if (pair.Length == 2)
                    rules[pair[0].ToLowerInvariant()] = pair[1];
            }
        }

        rules[name] = value;
        element.SetAttribute("style", String.Join("; ", rules.Select(x => $"{x.Key}: {x.Value}")));
    }

    private static void RemoveNode(INode? node)
    {
        node?.Parent?.RemoveChild(node);
    }

    private readonly record struct HtmlOptions(Boolean AllowImages);
}
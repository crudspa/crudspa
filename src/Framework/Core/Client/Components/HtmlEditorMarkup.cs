using Crudspa.Framework.Core.Shared.Markup;

namespace Crudspa.Framework.Core.Client.Components;

public static class HtmlEditorMarkup
{
    public static String? Normalize(String? html, Boolean allowImages = false) => ProseHtmlNormalizer.Normalize(html, allowImages);

    public static String? NormalizeForPaste(String? html, Boolean allowImages = false) => ProseHtmlNormalizer.NormalizeForPaste(html, allowImages);

    public static String? NormalizeForStorage(String? html, Boolean allowImages = false) => ProseHtmlNormalizer.NormalizeForStorage(html, allowImages);

    public static String? SanitizeEditorHtml(String? html, Boolean allowImages = false) => ProseHtmlNormalizer.NormalizeForStorage(html, allowImages);
}
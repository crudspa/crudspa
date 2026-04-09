using AngleSharp.Dom;
using AngleSharp.Html.Dom;
using Crudspa.Framework.Core.Shared.Markup;
using Ganss.Xss;
using Microsoft.AspNetCore.WebUtilities;
using IHtmlSanitizer = Crudspa.Framework.Core.Server.Contracts.Behavior.IHtmlSanitizer;

namespace Crudspa.Framework.Core.Server.Services;

public class HtmlSanitizerCore : IHtmlSanitizer
{
    private readonly String _currentHost;
    private readonly HtmlSanitizer _sanitizer;

    public HtmlSanitizerCore(IHttpContextAccessor httpContextAccessor)
    {
        _currentHost = httpContextAccessor.HttpContext?.Request.Host.Host ?? "localhost";

        _sanitizer = new(new()
        {
            AllowedAttributes = new HashSet<String>(ProseHtmlContract.SanitizerAllowedAttributes, StringComparer.OrdinalIgnoreCase),
            AllowedSchemes = new HashSet<String>(ProseHtmlContract.SanitizerAllowedSchemes, StringComparer.OrdinalIgnoreCase),
            AllowedTags = new HashSet<String>(ProseHtmlContract.AllowedTags, StringComparer.OrdinalIgnoreCase),
            AllowedCssProperties = new HashSet<String>(ProseHtmlContract.AllowedStyleProperties, StringComparer.OrdinalIgnoreCase),
        });

        _sanitizer.PostProcessNode += (_, args) =>
        {
            if (args.Node is not IHtmlAnchorElement anchor)
                return;

            var host = anchor.HostName.ToLowerInvariant();
            var isExternal = host.HasSomething()
                && !host.EndsWith(_currentHost)
                && !host.IsBasically("localhost");

            if (isExternal)
            {
                anchor.Target = "_blank";
                anchor.RelationList.Add("external");
                anchor.RelationList.Add("nofollow");
            }
            else
            {
                anchor.RemoveAttribute("rel");
                anchor.RemoveAttribute("target");
            }

            if (Uri.TryCreate(anchor.Href, UriKind.Absolute, out var absoluteUri))
            {
                var filteredQuery = QueryHelpers.ParseQuery(absoluteUri.Query)
                    .Select(x => new
                    {
                        Key = x.Key.TrimStart('?'),
                        x.Value,
                    })
                    .Where(x => x.Key.HasSomething())
                    .Where(x => x.Key is not "utm_source" and not "utm_medium" and not "utm_campaign" and not "utm_term" and not "utm_content")
                    .SelectMany(x => x.Value, (pair, value) => new KeyValuePair<String, String?>(pair.Key, value))
                    .ToList();

                var builder = new UriBuilder(absoluteUri)
                {
                    Query = QueryString.Create(filteredQuery).Value?.TrimStart('?') ?? String.Empty,
                };

                anchor.Href = builder.Uri.ToString();
            }

            var href = anchor.Href.RemoveFirst("about:///");

            if (href.EndsWith('?'))
                href = href[..^1];

            anchor.Href = href;
        };
    }

    public String? Sanitize(String? html, String? baseUrl = null)
    {
        var normalized = ProseHtmlNormalizer.NormalizeForStorage(html);

        if (ProseHtmlNormalizer.IsEmpty(normalized))
            return null;

        var sanitized = _sanitizer.Sanitize(normalized!, baseUrl ?? String.Empty);

        return ProseHtmlNormalizer.IsEmpty(sanitized)
            ? null
            : sanitized;
    }
}
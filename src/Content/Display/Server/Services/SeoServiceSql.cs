using System.Net;
using System.Text;
using System.Xml.Linq;
using Crudspa.Framework.Core.Server.Contracts;
using Microsoft.AspNetCore.Http;

namespace Crudspa.Content.Display.Server.Services;

public class SeoServiceSql(
    IServerConfigService configService,
    ICacheService cacheService)
    : ISeoService
{
    private String Connection => configService.Fetch().Database;
    private Guid PortalId => configService.Fetch().PortalId;

    public async Task<SeoPage> FetchPage(String? path, String requestBaseUrl)
    {
        var portal = await FetchPortal();
        if (portal is null)
            return NotFound(requestBaseUrl, path);

        var routes = await FetchRoutes();
        var normalizedPath = NormalizePath(path);
        var route = FindRoute(routes, normalizedPath);

        if (route is null)
            return NotFound(ResolveBaseUrl(portal, requestBaseUrl), normalizedPath);

        var baseUrl = ResolveBaseUrl(portal, requestBaseUrl);
        var page = new SeoPage
        {
            Found = true,
            Title = BuildTitle(portal, route),
            Description = route.SeoDescription.HasSomething() ? route.SeoDescription : portal.SeoDescription,
            Keywords = portal.SeoKeywords,
            CanonicalUrl = BuildCanonicalUrl(baseUrl, CanonicalPath(route)),
            ImageUrl = BuildAbsoluteUrl(baseUrl, portal.SeoImageFile.FetchUrl(1200)),
        };

        page.BodyHtml = await BuildBodyHtml(portal, route, routes);
        return page;
    }

    public async Task<String> FetchRobotsText(String requestBaseUrl)
    {
        var portal = await FetchPortal();
        var baseUrl = ResolveBaseUrl(portal, requestBaseUrl);

        return String.Join('\n',
            "User-agent: *",
            "Allow: /",
            $"Sitemap: {BuildAbsoluteUrl(baseUrl, "/sitemap.xml")}",
            String.Empty);
    }

    public async Task<String> FetchSitemapXml(String requestBaseUrl)
    {
        var portal = await FetchPortal();
        var routes = await FetchRoutes();
        var baseUrl = ResolveBaseUrl(portal, requestBaseUrl);
        var ns = XNamespace.Get("http://www.sitemaps.org/schemas/sitemap/0.9");
        var urls = routes
            .Where(x => x.Mapable)
            .Select(CanonicalPath)
            .DefaultIfEmpty("/")
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .OrderBy(x => x.IsBasically("/") ? 0 : 1)
            .ThenBy(x => x, StringComparer.OrdinalIgnoreCase)
            .Select(x => new XElement(ns + "url", new XElement(ns + "loc", BuildCanonicalUrl(baseUrl, x))));

        return new XDocument(new XElement(ns + "urlset", urls)).ToString(SaveOptions.DisableFormatting);
    }

    public static String BuildRequestBaseUrl(HttpRequest request)
    {
        return $"{request.Scheme}://{request.Host}";
    }

    private async Task<ContentPortal?> FetchPortal()
    {
        var cacheKey = String.Format(CacheKeys.PortalSeo, PortalId);
        return await cacheService.GetOrAdd<ContentPortal>(cacheKey, async () =>
            await ContentPortalSelect.Execute(Connection, PortalId));
    }

    private async Task<IList<SeoRoute>> FetchRoutes()
    {
        var cacheKey = String.Format(CacheKeys.PortalSeoRoutes, PortalId);
        var routes = await cacheService.GetOrAdd<IList<SeoRoute>>(cacheKey, async () =>
            await SeoRouteSelect.Execute(Connection, PortalId));

        return routes ?? [];
    }

    private async Task<String> BuildBodyHtml(ContentPortal portal, SeoRoute route, IList<SeoRoute> routes)
    {
        var builder = new StringBuilder();

        builder.Append("<main class=\"c-seo-page\" aria-hidden=\"true\">");
        builder.Append("<nav aria-label=\"Primary\">");

        foreach (var navRoute in routes.Where(x => x.Navigable && x.Path.Count(y => y == '/') == 1).OrderBy(x => x.Path))
            builder.Append("<a href=\"").Append(HtmlAttribute(CanonicalPath(navRoute))).Append("\">").Append(Html(navRoute.Title)).Append("</a>");

        builder.Append("</nav>");
        builder.Append("<h1>").Append(Html(route.PageTitle ?? route.Title ?? portal.SeoTitle ?? portal.Portal.Title ?? String.Empty)).Append("</h1>");

        var hasBodyContent = false;

        if (route.PageId.HasValue)
        {
            var page = await PageRunSelectContent.Execute(Connection, new() { Id = route.PageId }, null);

            if (page is not null)
            {
                AppendPage(builder, page);
                hasBodyContent = true;
            }
        }

        if (!hasBodyContent && route.SeoDescription.HasSomething())
            builder.Append("<section><p>").Append(Html(route.SeoDescription!)).Append("</p></section>");

        builder.Append("</main>");
        return builder.ToString();
    }

    private static void AppendPage(StringBuilder builder, Page page)
    {
        foreach (var section in page.Sections.OrderBy(x => x.Ordinal))
        foreach (var element in section.Elements.OrderBy(x => x.Ordinal))
            AppendElement(builder, element);
    }

    private static void AppendElement(StringBuilder builder, SectionElement element)
    {
        if (element.As<TextElement>() is { } text && text.Text.HasSomething())
        {
            builder.Append("<section>").Append(text.Text).Append("</section>");
            return;
        }

        if (element.As<ImageElement>() is { } image && image.FileFile.Id.HasValue)
        {
            AppendImage(builder, image.FileFile);
            return;
        }

        if (element.As<ButtonElement>() is { } button)
        {
            AppendButton(builder, button.Button);
            return;
        }

        if (element.As<MultimediaElement>() is { } multimedia)
        {
            foreach (var item in multimedia.MultimediaItems.OrderBy(x => x.Ordinal))
            {
                if (item.MediaTypeIndex == MultimediaItem.MediaTypes.Text && item.Text.HasSomething())
                    builder.Append("<section>").Append(item.Text).Append("</section>");
                else if (item.MediaTypeIndex == MultimediaItem.MediaTypes.Image && item.ImageFile.Id.HasValue)
                    AppendImage(builder, item.ImageFile);
                else if (item.MediaTypeIndex == MultimediaItem.MediaTypes.Button)
                    AppendButton(builder, item.Button);
            }
        }
    }

    private static void AppendImage(StringBuilder builder, ImageFile image)
    {
        builder.Append("<img src=\"")
            .Append(HtmlAttribute(image.FetchUrl(1200)))
            .Append("\" alt=\"")
            .Append(HtmlAttribute(image.Caption ?? image.Name ?? String.Empty))
            .Append("\" />");
    }

    private static void AppendButton(StringBuilder builder, Button button)
    {
        if (button.Text.HasNothing())
            return;

        var href = button.Internal == true && button.Path.HasSomething()
            ? button.Path!
            : button.Path.HasSomething()
                ? button.Path!
                : "#";

        builder.Append("<a href=\"")
            .Append(HtmlAttribute(href))
            .Append("\">")
            .Append(Html(button.Text!))
            .Append("</a>");
    }

    private static SeoRoute? FindRoute(IList<SeoRoute> routes, String normalizedPath)
    {
        if (normalizedPath.IsBasically("/"))
            return routes.FirstOrDefault(x => x.IsDefault) ?? routes.FirstOrDefault();

        return routes.FirstOrDefault(x => x.Path.IsBasically(normalizedPath));
    }

    private static String BuildTitle(ContentPortal portal, SeoRoute route)
    {
        var siteTitle = portal.SeoTitle ?? portal.Portal.Title ?? String.Empty;
        var routeTitle = route.PageTitle ?? route.Title;

        if (route.IsDefault || routeTitle.HasNothing() || routeTitle.IsBasically(siteTitle))
            return siteTitle;

        return $"{routeTitle} | {siteTitle}";
    }

    private static SeoPage NotFound(String baseUrl, String? path)
    {
        return new()
        {
            Found = false,
            Title = "Not Found",
            CanonicalUrl = BuildCanonicalUrl(baseUrl, NormalizePath(path)),
        };
    }

    private static String NormalizePath(String? path)
    {
        if (path.HasNothing())
            return "/";

        var normalized = path!.Split('?', '#')[0].Trim();

        if (normalized.HasNothing() || normalized == "/")
            return "/";

        normalized = "/" + normalized.Trim('/');
        return normalized.ToLowerInvariant();
    }

    private static String ResolveBaseUrl(ContentPortal? portal, String requestBaseUrl)
    {
        return (portal?.CanonicalBaseUrl.HasSomething() == true ? portal.CanonicalBaseUrl! : requestBaseUrl).TrimEnd('/');
    }

    private static String BuildCanonicalUrl(String baseUrl, String? path)
    {
        var normalizedPath = NormalizePath(path);
        return normalizedPath == "/" ? baseUrl : $"{baseUrl}{normalizedPath}";
    }

    private static String CanonicalPath(SeoRoute route) => route.IsDefault ? "/" : route.Path;

    private static String? BuildAbsoluteUrl(String baseUrl, String? path)
    {
        if (path.HasNothing())
            return null;

        if (Uri.TryCreate(path, UriKind.Absolute, out _))
            return path;

        return $"{baseUrl}/{path!.TrimStart('/')}";
    }

    private static String Html(String value) => WebUtility.HtmlEncode(value);
    private static String HtmlAttribute(String value) => WebUtility.HtmlEncode(value);
}
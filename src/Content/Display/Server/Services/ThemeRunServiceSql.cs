using Crudspa.Content.Display.Server.Contracts;
using Crudspa.Content.Display.Server.Extensions;
using Crudspa.Framework.Core.Server.Contracts;

namespace Crudspa.Content.Display.Server.Services;

public class ThemeRunServiceSql(
    IHostEnvironment hostingEnvironment,
    ICacheService cacheService,
    IServerConfigService configService,
    IStylesRunService stylesRunService,
    ICssService cssService,
    ILogger<ThemeRunServiceSql> logger)
    : IThemeRunService
{
    public async Task<String?> Fetch(Guid portalId, Int32 revision, String build)
    {
        if (hostingEnvironment.IsDevelopment())
        {
            var styles = await stylesRunService.FetchForPortal(portalId);

            if (styles is null)
                return null;

            var devRevision = styles.Revision.GetValueOrDefault();
            var devBuild = cssService.Tag(build);

            return await Cache(
                StylesKeys.Build(portalId, devRevision, devBuild),
                async () => await Build(styles));
        }

        return await Cache(
            StylesKeys.Build(portalId, revision, cssService.Tag(build)),
            async () =>
            {
                var styles = await stylesRunService.FetchForPortal(portalId);
                return styles is null ? null : await Build(styles);
            });
    }

    public async Task<Int32?> FetchRevision(Guid portalId)
    {
        return await stylesRunService.FetchStyleRevision(portalId);
    }

    public async Task<String?> Preview(Guid portalId, String scope)
    {
        var build = configService.Fetch().BuildNumber;

        if (hostingEnvironment.IsDevelopment())
        {
            var styles = await stylesRunService.FetchForPortal(portalId);

            if (styles is null)
                return null;

            var devRevision = styles.Revision.GetValueOrDefault();
            var devBuild = cssService.Tag(build);

            return await Cache(
                StylesKeys.Preview(portalId, devRevision, devBuild, scope),
                async () => await Build(styles, scope));
        }

        var revision = await FetchRevision(portalId);

        if (!revision.HasValue)
            return null;

        return await Cache(
            StylesKeys.Preview(portalId, revision.Value, cssService.Tag(build), scope),
            async () =>
            {
                var styles = await stylesRunService.FetchForPortal(portalId);
                return styles is null ? null : await Build(styles, scope);
            });
    }

    public async Task Warm(Guid portalId)
    {
        try
        {
            var styles = await stylesRunService.FetchForPortal(portalId);

            if (styles is null)
                return;

            var build = cssService.Tag(configService.Fetch().BuildNumber);
            var revision = styles.Revision.GetValueOrDefault();
            var preview = PreviewCss.Scope(portalId);

            await Cache(
                StylesKeys.Build(portalId, revision, build),
                async () => await Build(styles));

            await Cache(
                StylesKeys.Preview(portalId, revision, build, preview),
                async () => await Build(styles, preview));
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Exception while warming styles cache. PortalId: {portalId}", portalId);
        }
    }

    private async Task<String?> Build(PortalStyles styles, String? preview = null)
    {
        var bundle = await cssService.Bundle();
        var selector = preview.HasSomething() ? PreviewCss.Selector(preview) : null;
        var theme = await cssService.Theme(selector ?? CssPaths.Root, styles.ToScssVariables());

        if (bundle.HasNothing() || theme.HasNothing())
            return null;

        var body = selector.HasSomething()
            ? bundle.ScopeCss(selector)
            : bundle;

        return $"{styles.ToFontFaceCss()}{body}{theme}";
    }

    private async Task<String?> Cache(String key, Func<Task<String?>> build)
    {
        var css = cacheService.GetValue<String>(key);

        if (css.HasSomething())
            return css;

        css = await build();

        if (css.HasSomething())
            cacheService.AddValue(key, css);

        return css;
    }
}
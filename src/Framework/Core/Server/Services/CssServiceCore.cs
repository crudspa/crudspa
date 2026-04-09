using System.Text;
using System.Text.RegularExpressions;
using Crudspa.Framework.Core.Server.Contracts;

namespace Crudspa.Framework.Core.Server.Services;

public class CssServiceCore(
    IHostEnvironment hostingEnvironment,
    ISassCompiler sassCompiler,
    ICacheService cacheService,
    IServerConfigService configService)
    : ICssService
{
    public async Task<String?> Fetch(String build)
    {
        return await Cache(String.Format(CacheKeys.Css, Tag(build)), async () => await BuildHost());
    }

    public async Task<String?> Bundle()
    {
        if (!CanCache)
            return await CompileBundle();

        return await Cache(CssKeys.Bundle(configService.Fetch().BuildNumber), async () => await CompileBundle());
    }

    public async Task<String?> Theme(String selector, String? overrides = null)
    {
        var includePaths = IncludePaths();
        var variables = await Vars();

        if (includePaths.Count == 0 || variables.Count == 0)
            return null;

        return sassCompiler.Compile(ThemeScss(selector, variables, overrides), includePaths);
    }

    public String Tag(String build)
    {
        return hostingEnvironment.IsDevelopment()
            ? $"{build}-{Stamp()}"
            : build;
    }

    private Boolean CanCache => !hostingEnvironment.IsDevelopment();

    private async Task<String?> BuildHost()
    {
        var bundle = await Bundle();
        var theme = await Theme(CssPaths.Root);

        if (bundle.HasNothing() || theme.HasNothing())
            return null;

        return $"{bundle}{theme}";
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

    private async Task<String?> CompileBundle()
    {
        var scss = await File.ReadAllTextAsync(HostPath());
        return sassCompiler.Compile(scss, IncludePaths());
    }

    private String? DefaultsPath()
    {
        if (!hostingEnvironment.IsDevelopment())
        {
            var path = Path.Combine(CssPaths.StylesPath(AppDomain.CurrentDomain.BaseDirectory), CssPaths.DefaultsFile);
            return File.Exists(path) ? path : null;
        }

        var sourceRoot = SourceRoot();

        if (sourceRoot.HasNothing())
            return null;

        var sourcePath = CssPaths.CoreDefaults(sourceRoot);
        return File.Exists(sourcePath) ? sourcePath : null;
    }

    private String HostPath()
    {
        var root = hostingEnvironment.IsDevelopment()
            ? CssPaths.StylesPath(hostingEnvironment.ContentRootPath)
            : CssPaths.StylesPath(AppDomain.CurrentDomain.BaseDirectory);

        return Path.Combine(root, CssPaths.HostName(hostingEnvironment));
    }

    private List<String> IncludePaths()
    {
        if (!hostingEnvironment.IsDevelopment())
            return [CssPaths.StylesPath(AppDomain.CurrentDomain.BaseDirectory)];

        var sourceRoot = SourceRoot()
            ?? throw new DirectoryNotFoundException("Could not locate the source root for SCSS compilation.");

        return Directory.GetDirectories(sourceRoot, CssPaths.Styles, SearchOption.AllDirectories)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }

    private String? SourceRoot()
    {
        var directory = new DirectoryInfo(hostingEnvironment.ContentRootPath);

        while (directory is not null)
        {
            if (directory.Name.Equals(CssPaths.SourceRoot, StringComparison.OrdinalIgnoreCase))
                return directory.FullName;

            directory = directory.Parent;
        }

        return null;
    }

    private String Stamp()
    {
        return IncludePaths()
            .SelectMany(path => Directory.GetFiles(path, "*.scss", SearchOption.AllDirectories))
            .Select(path => File.GetLastWriteTimeUtc(path).Ticks)
            .DefaultIfEmpty(0)
            .Max()
            .ToString();
    }

    private static String ThemeScss(String selector, IReadOnlyCollection<String> variables, String? overrides)
    {
        var scss = new StringBuilder();

        if (overrides.HasSomething())
            scss.AppendLine(overrides);

        scss.AppendLine($"@import '{CssPaths.DefaultsImport}';");
        scss.Append(selector);
        scss.AppendLine(" {");

        foreach (var variable in variables)
        {
            scss.Append("  --");
            scss.Append(Kebab(variable));
            scss.Append(": #{$");
            scss.Append(variable);
            scss.AppendLine("};");
        }

        scss.AppendLine("}");

        return scss.ToString();
    }

    private async Task<IReadOnlyCollection<String>> Vars()
    {
        var path = DefaultsPath();

        if (path.HasNothing() || !File.Exists(path))
            return [];

        if (!CanCache)
            return await ReadVars(path);

        var variables = cacheService.GetValue<List<String>>(CssKeys.Vars(configService.Fetch().BuildNumber));

        if (variables is not null)
            return variables;

        variables = (await ReadVars(path)).ToList();
        cacheService.AddValue(CssKeys.Vars(configService.Fetch().BuildNumber), variables);

        return variables;
    }

    private static String Kebab(String input)
    {
        return Regex.Replace(input, "([a-z0-9])([A-Z])", "$1-$2").ToLowerInvariant();
    }

    private static async Task<IReadOnlyCollection<String>> ReadVars(String path)
    {
        return Regex.Matches(await File.ReadAllTextAsync(path), "^\\$([A-Za-z0-9]+)\\s*:", RegexOptions.Multiline)
            .Select(x => x.Groups[1].Value)
            .Distinct(StringComparer.OrdinalIgnoreCase)
            .ToList();
    }
}
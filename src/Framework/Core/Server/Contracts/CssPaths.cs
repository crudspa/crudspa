namespace Crudspa.Framework.Core.Server.Contracts;

public static class CssPaths
{
    public const String Type = "text/css";
    public const String DefaultsFile = "defaults.scss";
    public const String DefaultsImport = "defaults";
    public const String FrameworkPath = @"Framework\Core\Server";
    public const String Root = ":root";
    public const String SourceRoot = "src";
    public const String Styles = "Styles";

    public static String CoreDefaults(String sourceRoot)
    {
        return Path.Combine(sourceRoot, FrameworkPath, Styles, DefaultsFile);
    }

    public static String HostName(IHostEnvironment hostingEnvironment)
    {
        return hostingEnvironment.IsDevelopment()
            ? HostNameFromPath(hostingEnvironment.ContentRootPath)
            : HostNameFromApp(hostingEnvironment.ApplicationName);
    }

    public static String StylesPath(String rootPath)
    {
        return Path.Combine(rootPath, Styles);
    }

    private static String HostNameFromApp(String applicationName)
    {
        if (applicationName.HasNothing())
            throw new("Could not infer the host stylesheet name because the application name is missing.");

        var parts = applicationName.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);

        if (parts.Length < 4 || !parts[^1].Equals("Server", StringComparison.OrdinalIgnoreCase))
            throw new($"Could not infer the host stylesheet name from application '{applicationName}'.");

        return $"{parts[^3].ToLowerInvariant()}-{parts[^2].ToLowerInvariant()}.scss";
    }

    private static String HostNameFromPath(String contentRootPath)
    {
        var directory = new DirectoryInfo(contentRootPath);
        var moduleName = directory.Parent?.Name;
        var areaName = directory.Parent?.Parent?.Name;

        if (areaName.HasNothing() || moduleName.HasNothing())
            throw new($"Could not infer the host stylesheet name from content root '{contentRootPath}'.");

        return $"{areaName.ToLowerInvariant()}-{moduleName.ToLowerInvariant()}.scss";
    }
}
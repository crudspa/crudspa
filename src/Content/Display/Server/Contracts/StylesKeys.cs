using Crudspa.Framework.Core.Server.Contracts;

namespace Crudspa.Content.Display.Server.Contracts;

public static class StylesKeys
{
    public static String Build(Guid portalId, Int32 revision, String build)
    {
        return String.Format(CacheKeys.Styles, portalId, revision, build);
    }

    public static String Preview(Guid portalId, Int32 revision, String build, String scope)
    {
        return Build(portalId, revision, PreviewBuild(build, scope));
    }

    public static String PreviewBuild(String build, String scope)
    {
        return $"{build}-preview-{scope}";
    }

    public static String Revision(Guid? portalId)
    {
        return $"Styles-Revision-{portalId.GetValueOrDefault():D}";
    }
}
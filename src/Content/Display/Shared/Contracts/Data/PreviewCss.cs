namespace Crudspa.Content.Display.Shared.Contracts.Data;

public static class PreviewCss
{
    public const String ClassPrefix = "c-theme-preview-";
    public const String ScopePrefix = "portal-";

    public static String Class(String scope)
    {
        return $"{ClassPrefix}{scope}";
    }

    public static String Scope(Guid portalId)
    {
        return $"{ScopePrefix}{portalId:N}";
    }

    public static String Selector(String scope)
    {
        return $".{Class(scope)}";
    }
}
namespace Crudspa.Framework.Core.Server.Contracts;

public static class CssKeys
{
    public static String Bundle(String build)
    {
        return $"Styles-Bundle-Build-{build}";
    }

    public static String Vars(String build)
    {
        return $"Styles-Vars-Build-{build}";
    }
}
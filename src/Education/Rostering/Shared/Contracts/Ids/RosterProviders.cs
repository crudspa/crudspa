namespace Crudspa.Education.Rostering.Shared.Contracts.Ids;

public static class RosterProviders
{
    public const String Manual = "manual";
    public const String Clever = "clever";
    public const String ClassLink = "classlink";
    public const String OneRoster = "oneroster";

    public static readonly IReadOnlyDictionary<String, String> Names = new Dictionary<String, String>
    {
        [Manual] = "Manual / Bespoke",
        [Clever] = "Clever",
        [ClassLink] = "ClassLink",
        [OneRoster] = "OneRoster",
    };

    public static Boolean IsAutomated(String? provider) =>
        provider is Clever or ClassLink or OneRoster;

    public static Boolean UsesTenant(String? provider) =>
        provider is Clever or ClassLink;
}
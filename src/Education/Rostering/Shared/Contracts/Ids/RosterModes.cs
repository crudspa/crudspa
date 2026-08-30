namespace Crudspa.Education.Rostering.Shared.Contracts.Ids;

public static class RosterModes
{
    public const String Disabled = "disabled";
    public const String Shadow = "shadow";
    public const String Authoritative = "authoritative";

    public static readonly IReadOnlyDictionary<String, String> Names = new Dictionary<String, String>
    {
        [Disabled] = "Disabled",
        [Shadow] = "Shadow",
        [Authoritative] = "Authoritative",
    };

    public static readonly IReadOnlyDictionary<String, String> EditNames = new Dictionary<String, String>
    {
        [Disabled] = "Disabled",
        [Shadow] = "Shadow",
    };
}
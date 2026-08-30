namespace Crudspa.Framework.Auth.Shared.Contracts.Ids;

public static class AuthProviders
{
    public const String PasswordEmailCode = "password-email-code";
    public const String EmailCode = "email-code";
    public const String StudentCode = "student-code";
    public const String Clever = "clever";
    public const String ClassLink = "classlink";

    public static readonly IReadOnlyDictionary<String, String> Names = new Dictionary<String, String>
    {
        [PasswordEmailCode] = "Password + Email Code",
        [EmailCode] = "Email Code (Passwordless)",
        [StudentCode] = "Student Code (Legacy)",
        [Clever] = "Clever",
        [ClassLink] = "ClassLink",
    };

    public static Boolean IsExternal(String? provider) =>
        provider is Clever or ClassLink;
}
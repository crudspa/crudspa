using Crudspa.Framework.Core.Server.Contracts.Behavior;
using System.Text;

namespace Crudspa.Framework.Auth.Server.Extensions;

public static class AuthIdentityEx
{
    public static Byte[] ComputeIdentityKey(this ICryptographyService cryptographyService, String provider, String issuer, String subject)
    {
        var key = new StringBuilder();
        Append(key, provider.ToLowerInvariant());
        Append(key, issuer);
        Append(key, subject);
        return cryptographyService.ComputeHash(key.ToString());
    }

    private static void Append(StringBuilder key, String value)
    {
        key.Append(Encoding.UTF8.GetByteCount(value));
        key.Append(':');
        key.Append(value);
    }
}
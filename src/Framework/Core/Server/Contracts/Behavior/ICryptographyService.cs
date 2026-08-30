namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ICryptographyService
{
    Guid GetRandomGuid();
    Int32 GetRandomInt(Int32 min, Int32 max);
    Byte[] GetRandomSalt();
    Byte[] ComputeHash(String input);
    Byte[] ComputeHash(String input, Byte[] salt);
    String Protect(String value);
    String Unprotect(String value);
}
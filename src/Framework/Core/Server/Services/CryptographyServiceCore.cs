using System.Security.Cryptography;
using System.Text;

namespace Crudspa.Framework.Core.Server.Services;

public class CryptographyServiceCore(IConfiguration configuration) : ICryptographyService
{
    private const String Prefix = "v1:";

    public Guid GetRandomGuid()
    {
        var generator = RandomNumberGenerator.Create();
        var bytes = new Byte[16];
        generator.GetBytes(bytes);
        return new(bytes);
    }

    public Int32 GetRandomInt(Int32 min, Int32 max)
    {
        return RandomNumberGenerator.GetInt32(min, max);
    }

    public Byte[] GetRandomSalt()
    {
        var generator = RandomNumberGenerator.Create();
        var salt = new Byte[32];
        generator.GetBytes(salt);
        return salt;
    }

    public Byte[] ComputeHash(String input, Byte[] salt)
    {
        var computer = SHA256.Create();
        var bytes = Encoding.UTF8.GetBytes(input);
        var salted = new Byte[bytes.Length + salt.Length];

        bytes.CopyTo(salted, 0);
        salt.CopyTo(salted, bytes.Length);

        return computer.ComputeHash(salted);
    }

    public Byte[] ComputeHash(String input)
    {
        return ComputeHash(input, []);
    }

    public String Protect(String value)
    {
        if (value.HasNothing() || value.StartsWith(Prefix, StringComparison.Ordinal))
            return value;

        var key = Key();
        if (key is null)
            return value;

        var nonce = RandomNumberGenerator.GetBytes(12);
        var tag = new Byte[16];
        var plain = Encoding.UTF8.GetBytes(value);
        var cipher = new Byte[plain.Length];
        using var aes = new AesGcm(key, tag.Length);
        aes.Encrypt(nonce, plain, cipher, tag);
        return Prefix + Convert.ToBase64String([.. nonce, .. tag, .. cipher]);
    }

    public String Unprotect(String value)
    {
        if (value.HasNothing() || !value.StartsWith(Prefix, StringComparison.Ordinal))
            return value;

        var key = Key() ?? throw new InvalidOperationException("Security Encryption Key is required.");
        var payload = Convert.FromBase64String(value[Prefix.Length..]);
        var plain = new Byte[payload.Length - 28];
        using var aes = new AesGcm(key, 16);
        aes.Decrypt(payload[..12], payload[28..], payload[12..28], plain);
        return Encoding.UTF8.GetString(plain);
    }

    private Byte[]? Key()
    {
        var value = configuration["Security.EncryptionKey"];
        if (value.HasNothing())
            return null;

        var key = Convert.FromBase64String(value);
        return key.Length == 32 ? key : throw new InvalidOperationException("Security Encryption Key must contain 32 bytes.");
    }
}
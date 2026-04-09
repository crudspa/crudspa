using System.Security.Cryptography;
using System.Text;

namespace Crudspa.Framework.Core.Server.Services;

public class CryptographyServiceCore : ICryptographyService
{
    public Guid GetRandomGuid()
    {
        var generator = RandomNumberGenerator.Create();
        var bytes = new Byte[16];
        generator.GetBytes(bytes);
        return new(bytes);
    }

    public Int32 GetRandomInt(Int32 min, Int32 max)
    {
        var generator = RandomNumberGenerator.Create();
        var bytes = new Byte[4];
        generator.GetBytes(bytes);
        var seed = BitConverter.ToInt32(bytes, 0);
        return new Random(seed).Next(min, max);
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
}
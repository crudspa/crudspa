using System.Reflection;
using System.Resources;

namespace Crudspa.Framework.Core.Server.Services;

public class EmbeddedResourceServiceCore : IEmbeddedResourceService
{
    public async Task<String> ReadText(Assembly assembly, String resourceName)
    {
        await using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
            throw new MissingManifestResourceException($"The embedded resource '{resourceName}' could not be found in the assembly '{assembly.FullName}'.");

        using var reader = new StreamReader(stream);
        return await reader.ReadToEndAsync();
    }

    public async Task<Byte[]> ReadBytes(Assembly assembly, String resourceName)
    {
        await using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
            throw new MissingManifestResourceException($"The embedded resource '{resourceName}' could not be found in the assembly '{assembly.FullName}'.");

        var bytes = new Byte[stream.Length];
        _ = await stream.ReadAsync(bytes, 0, (Int32)stream.Length);
        return bytes;
    }

    public Byte[] ReadBytesNow(Assembly assembly, String resourceName)
    {
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
            throw new MissingManifestResourceException($"The embedded resource '{resourceName}' could not be found in the assembly '{assembly.FullName}'.");

        var bytes = new Byte[stream.Length];
        _ = stream.Read(bytes, 0, (Int32)stream.Length);
        return bytes;
    }
}
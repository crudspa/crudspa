using System.Collections.Concurrent;
using System.Collections.Frozen;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Resources;
using Microsoft.AspNetCore.Mvc;

namespace Crudspa.Framework.Core.Server.Controllers;

public abstract class EmbeddedResourceController : ControllerBase
{
    protected ActionResult EmbeddedResourceFile(EmbeddedResourceFileCache cache, String fileName)
    {
        if (!cache.TryGet(fileName, out var file))
            return NotFound();

        Response.SetEmbeddedResourceFileHeaders(file.FileName);

        return new FileContentResult(file.Bytes, file.ContentType);
    }
}

public sealed class EmbeddedResourceFileCache(Assembly assembly, String resourceNamespace)
{
    private readonly ConcurrentDictionary<String, Lazy<EmbeddedResourceFile>> _files = new(StringComparer.Ordinal);
    private readonly FrozenSet<String> _fileNames = assembly.GetManifestResourceNames()
        .Where(resourceName => resourceName.StartsWith(resourceNamespace, StringComparison.Ordinal))
        .Select(resourceName => resourceName[resourceNamespace.Length..])
        .ToFrozenSet(StringComparer.Ordinal);

    internal Boolean TryGet(String fileName, [NotNullWhen(true)] out EmbeddedResourceFile? file)
    {
        file = null;

        if (!_fileNames.Contains(fileName))
            return false;

        file = _files.GetOrAdd(fileName, name => new(() => Load(name))).Value;
        return true;
    }

    private EmbeddedResourceFile Load(String fileName)
    {
        var resourceName = $"{resourceNamespace}{fileName}";
        using var stream = assembly.GetManifestResourceStream(resourceName);

        if (stream is null)
            throw new MissingManifestResourceException($"The embedded resource '{resourceName}' was registered but could not be opened from the assembly '{assembly.FullName}'.");

        var bytes = new Byte[checked((Int32)stream.Length)];
        stream.ReadExactly(bytes);

        return new(fileName, fileName.GetExtension().ToMimeType(), bytes);
    }
}

internal sealed record EmbeddedResourceFile(String FileName, String ContentType, Byte[] Bytes);
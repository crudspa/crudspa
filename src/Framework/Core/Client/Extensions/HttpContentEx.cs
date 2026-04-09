using System.Net;

namespace Crudspa.Framework.Core.Client.Extensions;

public class HttpContentEx(Stream content, Int64 totalSize, Action<Int64, Int64> progress) : HttpContent
{
    private const Int32 BufferSize = 4096;

    protected override async Task SerializeToStreamAsync(Stream stream, TransportContext? context)
    {
        var buffer = new Byte[BufferSize];
        Int64 uploaded = 0;

        Int32 bytesRead;
        while ((bytesRead = await content.ReadAsync(buffer.AsMemory(0, BufferSize))) != 0)
        {
            await stream.WriteAsync(buffer.AsMemory(0, bytesRead));
            uploaded += bytesRead;
            progress?.Invoke(uploaded, totalSize);
        }
    }

    protected override Boolean TryComputeLength(out Int64 length)
    {
        length = totalSize;
        return true;
    }

    protected override void Dispose(Boolean disposing)
    {
        if (disposing)
            content.Dispose();

        base.Dispose(disposing);
    }
}
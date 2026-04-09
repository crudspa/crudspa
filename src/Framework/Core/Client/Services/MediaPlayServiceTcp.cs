namespace Crudspa.Framework.Core.Client.Services;

public class MediaPlayServiceTcp(IProxyWrappers proxyWrappers) : IMediaPlayService
{
    public async Task<Response> Add(Request<MediaPlay> request) =>
        await proxyWrappers.Send("MediaPlayAdd", request);
}
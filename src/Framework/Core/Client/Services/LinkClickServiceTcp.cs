namespace Crudspa.Framework.Core.Client.Services;

public class LinkClickServiceTcp(IProxyWrappers proxyWrappers) : ILinkClickService
{
    public async Task<Response> Add(Request<LinkClick> request) =>
        await proxyWrappers.Send("LinkClickAdd", request);
}
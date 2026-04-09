namespace Crudspa.Framework.Core.Client.Services;

public class AddressServiceTcp(IProxyWrappers proxyWrappers) : IAddressService
{
    public async Task<Response<IList<Named>>> FetchUsaStateNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Named>>("AddressFetchUsaStateNames", request);
}
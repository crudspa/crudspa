
namespace Crudspa.Content.Design.Client.Services;

public class PublicationGroupServiceTcp(IProxyWrappers proxyWrappers) : IPublicationGroupService
{
    public async Task<Response<IList<PublicationGroup>>> FetchForPortal(Request<Portal> request) =>
        await proxyWrappers.Send<IList<PublicationGroup>>("PublicationGroupFetchForPortal", request);

    public async Task<Response<PublicationGroup?>> Fetch(Request<PublicationGroup> request) =>
        await proxyWrappers.Send<PublicationGroup?>("PublicationGroupFetch", request);

    public async Task<Response<PublicationGroup?>> Add(Request<PublicationGroup> request) =>
        await proxyWrappers.Send<PublicationGroup?>("PublicationGroupAdd", request);

    public async Task<Response> Save(Request<PublicationGroup> request) =>
        await proxyWrappers.Send("PublicationGroupSave", request);

    public async Task<Response> Remove(Request<PublicationGroup> request) =>
        await proxyWrappers.Send("PublicationGroupRemove", request);

    public async Task<Response> SaveOrder(Request<IList<PublicationGroup>> request) =>
        await proxyWrappers.Send("PublicationGroupSaveOrder", request);
}
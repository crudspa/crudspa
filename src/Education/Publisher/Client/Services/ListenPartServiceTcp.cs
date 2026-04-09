namespace Crudspa.Education.Publisher.Client.Services;

public class ListenPartServiceTcp(IProxyWrappers proxyWrappers) : IListenPartService
{
    public async Task<Response<IList<ListenPart>>> FetchForAssessment(Request<Assessment> request) =>
        await proxyWrappers.Send<IList<ListenPart>>("ListenPartFetchForAssessment", request);

    public async Task<Response<ListenPart?>> Fetch(Request<ListenPart> request) =>
        await proxyWrappers.Send<ListenPart?>("ListenPartFetch", request);

    public async Task<Response<ListenPart?>> Add(Request<ListenPart> request) =>
        await proxyWrappers.Send<ListenPart?>("ListenPartAdd", request);

    public async Task<Response> Save(Request<ListenPart> request) =>
        await proxyWrappers.Send("ListenPartSave", request);

    public async Task<Response> Remove(Request<ListenPart> request) =>
        await proxyWrappers.Send("ListenPartRemove", request);

    public async Task<Response> SaveOrder(Request<IList<ListenPart>> request) =>
        await proxyWrappers.Send("ListenPartSaveOrder", request);
}
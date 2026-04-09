namespace Crudspa.Education.Publisher.Client.Services;

public class ReadPartServiceTcp(IProxyWrappers proxyWrappers) : IReadPartService
{
    public async Task<Response<IList<ReadPart>>> FetchForAssessment(Request<Assessment> request) =>
        await proxyWrappers.Send<IList<ReadPart>>("ReadPartFetchForAssessment", request);

    public async Task<Response<ReadPart?>> Fetch(Request<ReadPart> request) =>
        await proxyWrappers.Send<ReadPart?>("ReadPartFetch", request);

    public async Task<Response<ReadPart?>> Add(Request<ReadPart> request) =>
        await proxyWrappers.Send<ReadPart?>("ReadPartAdd", request);

    public async Task<Response> Save(Request<ReadPart> request) =>
        await proxyWrappers.Send("ReadPartSave", request);

    public async Task<Response> Remove(Request<ReadPart> request) =>
        await proxyWrappers.Send("ReadPartRemove", request);

    public async Task<Response> SaveOrder(Request<IList<ReadPart>> request) =>
        await proxyWrappers.Send("ReadPartSaveOrder", request);
}
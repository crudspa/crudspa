namespace Crudspa.Education.Publisher.Client.Services;

public class VocabPartServiceTcp(IProxyWrappers proxyWrappers) : IVocabPartService
{
    public async Task<Response<IList<VocabPart>>> FetchForAssessment(Request<Assessment> request) =>
        await proxyWrappers.Send<IList<VocabPart>>("VocabPartFetchForAssessment", request);

    public async Task<Response<VocabPart?>> Fetch(Request<VocabPart> request) =>
        await proxyWrappers.Send<VocabPart?>("VocabPartFetch", request);

    public async Task<Response<VocabPart?>> Add(Request<VocabPart> request) =>
        await proxyWrappers.Send<VocabPart?>("VocabPartAdd", request);

    public async Task<Response> Save(Request<VocabPart> request) =>
        await proxyWrappers.Send("VocabPartSave", request);

    public async Task<Response> Remove(Request<VocabPart> request) =>
        await proxyWrappers.Send("VocabPartRemove", request);

    public async Task<Response> SaveOrder(Request<IList<VocabPart>> request) =>
        await proxyWrappers.Send("VocabPartSaveOrder", request);
}
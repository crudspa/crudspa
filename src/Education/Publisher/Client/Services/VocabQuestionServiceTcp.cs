namespace Crudspa.Education.Publisher.Client.Services;

public class VocabQuestionServiceTcp(IProxyWrappers proxyWrappers) : IVocabQuestionService
{
    public async Task<Response<IList<VocabQuestion>>> FetchForVocabPart(Request<VocabPart> request) =>
        await proxyWrappers.Send<IList<VocabQuestion>>("VocabQuestionFetchForVocabPart", request);

    public async Task<Response<VocabQuestion?>> Fetch(Request<VocabQuestion> request) =>
        await proxyWrappers.Send<VocabQuestion?>("VocabQuestionFetch", request);

    public async Task<Response<VocabQuestion?>> Add(Request<VocabQuestion> request) =>
        await proxyWrappers.Send<VocabQuestion?>("VocabQuestionAdd", request);

    public async Task<Response> Save(Request<VocabQuestion> request) =>
        await proxyWrappers.Send("VocabQuestionSave", request);

    public async Task<Response> Remove(Request<VocabQuestion> request) =>
        await proxyWrappers.Send("VocabQuestionRemove", request);

    public async Task<Response> SaveOrder(Request<IList<VocabQuestion>> request) =>
        await proxyWrappers.Send("VocabQuestionSaveOrder", request);
}
namespace Crudspa.Education.Publisher.Client.Services;

public class ReadQuestionServiceTcp(IProxyWrappers proxyWrappers) : IReadQuestionService
{
    public async Task<Response<IList<ReadQuestion>>> FetchForReadPart(Request<ReadPart> request) =>
        await proxyWrappers.Send<IList<ReadQuestion>>("ReadQuestionFetchForReadPart", request);

    public async Task<Response<ReadQuestion?>> Fetch(Request<ReadQuestion> request) =>
        await proxyWrappers.Send<ReadQuestion?>("ReadQuestionFetch", request);

    public async Task<Response<ReadQuestion?>> Add(Request<ReadQuestion> request) =>
        await proxyWrappers.Send<ReadQuestion?>("ReadQuestionAdd", request);

    public async Task<Response> Save(Request<ReadQuestion> request) =>
        await proxyWrappers.Send("ReadQuestionSave", request);

    public async Task<Response> Remove(Request<ReadQuestion> request) =>
        await proxyWrappers.Send("ReadQuestionRemove", request);

    public async Task<Response> SaveOrder(Request<IList<ReadQuestion>> request) =>
        await proxyWrappers.Send("ReadQuestionSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchReadQuestionCategoryNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ReadQuestionFetchReadQuestionCategoryNames", request);

    public async Task<Response<IList<Orderable>>> FetchReadQuestionTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ReadQuestionFetchReadQuestionTypeNames", request);
}
namespace Crudspa.Education.Publisher.Client.Services;

public class ListenQuestionServiceTcp(IProxyWrappers proxyWrappers) : IListenQuestionService
{
    public async Task<Response<IList<ListenQuestion>>> FetchForListenPart(Request<ListenPart> request) =>
        await proxyWrappers.Send<IList<ListenQuestion>>("ListenQuestionFetchForListenPart", request);

    public async Task<Response<ListenQuestion?>> Fetch(Request<ListenQuestion> request) =>
        await proxyWrappers.Send<ListenQuestion?>("ListenQuestionFetch", request);

    public async Task<Response<ListenQuestion?>> Add(Request<ListenQuestion> request) =>
        await proxyWrappers.Send<ListenQuestion?>("ListenQuestionAdd", request);

    public async Task<Response> Save(Request<ListenQuestion> request) =>
        await proxyWrappers.Send("ListenQuestionSave", request);

    public async Task<Response> Remove(Request<ListenQuestion> request) =>
        await proxyWrappers.Send("ListenQuestionRemove", request);

    public async Task<Response> SaveOrder(Request<IList<ListenQuestion>> request) =>
        await proxyWrappers.Send("ListenQuestionSaveOrder", request);

    public async Task<Response<IList<Orderable>>> FetchListenQuestionCategoryNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ListenQuestionFetchListenQuestionCategoryNames", request);

    public async Task<Response<IList<Orderable>>> FetchReadQuestionTypeNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("ListenQuestionFetchReadQuestionTypeNames", request);
}
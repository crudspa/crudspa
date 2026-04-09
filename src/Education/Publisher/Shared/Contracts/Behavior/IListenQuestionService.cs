namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IListenQuestionService
{
    Task<Response<IList<ListenQuestion>>> FetchForListenPart(Request<ListenPart> request);
    Task<Response<ListenQuestion?>> Fetch(Request<ListenQuestion> request);
    Task<Response<ListenQuestion?>> Add(Request<ListenQuestion> request);
    Task<Response> Save(Request<ListenQuestion> request);
    Task<Response> Remove(Request<ListenQuestion> request);
    Task<Response> SaveOrder(Request<IList<ListenQuestion>> request);
    Task<Response<IList<Orderable>>> FetchListenQuestionCategoryNames(Request request);
    Task<Response<IList<Orderable>>> FetchReadQuestionTypeNames(Request request);
}
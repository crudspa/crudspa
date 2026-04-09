namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IReadQuestionService
{
    Task<Response<IList<ReadQuestion>>> FetchForReadPart(Request<ReadPart> request);
    Task<Response<ReadQuestion?>> Fetch(Request<ReadQuestion> request);
    Task<Response<ReadQuestion?>> Add(Request<ReadQuestion> request);
    Task<Response> Save(Request<ReadQuestion> request);
    Task<Response> Remove(Request<ReadQuestion> request);
    Task<Response> SaveOrder(Request<IList<ReadQuestion>> request);
    Task<Response<IList<Orderable>>> FetchReadQuestionCategoryNames(Request request);
    Task<Response<IList<Orderable>>> FetchReadQuestionTypeNames(Request request);
}
namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IVocabQuestionService
{
    Task<Response<IList<VocabQuestion>>> FetchForVocabPart(Request<VocabPart> request);
    Task<Response<VocabQuestion?>> Fetch(Request<VocabQuestion> request);
    Task<Response<VocabQuestion?>> Add(Request<VocabQuestion> request);
    Task<Response> Save(Request<VocabQuestion> request);
    Task<Response> Remove(Request<VocabQuestion> request);
    Task<Response> SaveOrder(Request<IList<VocabQuestion>> request);
}
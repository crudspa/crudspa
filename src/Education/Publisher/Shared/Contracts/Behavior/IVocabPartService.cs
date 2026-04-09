namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IVocabPartService
{
    Task<Response<IList<VocabPart>>> FetchForAssessment(Request<Assessment> request);
    Task<Response<VocabPart?>> Fetch(Request<VocabPart> request);
    Task<Response<VocabPart?>> Add(Request<VocabPart> request);
    Task<Response> Save(Request<VocabPart> request);
    Task<Response> Remove(Request<VocabPart> request);
    Task<Response> SaveOrder(Request<IList<VocabPart>> request);
}
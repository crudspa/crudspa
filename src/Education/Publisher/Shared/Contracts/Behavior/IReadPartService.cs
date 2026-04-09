namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IReadPartService
{
    Task<Response<IList<ReadPart>>> FetchForAssessment(Request<Assessment> request);
    Task<Response<ReadPart?>> Fetch(Request<ReadPart> request);
    Task<Response<ReadPart?>> Add(Request<ReadPart> request);
    Task<Response> Save(Request<ReadPart> request);
    Task<Response> Remove(Request<ReadPart> request);
    Task<Response> SaveOrder(Request<IList<ReadPart>> request);
}
namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IListenPartService
{
    Task<Response<IList<ListenPart>>> FetchForAssessment(Request<Assessment> request);
    Task<Response<ListenPart?>> Fetch(Request<ListenPart> request);
    Task<Response<ListenPart?>> Add(Request<ListenPart> request);
    Task<Response> Save(Request<ListenPart> request);
    Task<Response> Remove(Request<ListenPart> request);
    Task<Response> SaveOrder(Request<IList<ListenPart>> request);
}
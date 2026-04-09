namespace Crudspa.Education.Student.Shared.Contracts.Behavior;

public interface IObjectiveProgressService
{
    Task<Response<IList<ObjectiveProgress>>> FetchAll(Request request);
    Task<ObjectiveProgress> Fetch(Request<Objective> request);
    Task<Response> AddCompleted(Request<ObjectiveCompleted> request);
}
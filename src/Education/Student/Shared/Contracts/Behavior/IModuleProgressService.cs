namespace Crudspa.Education.Student.Shared.Contracts.Behavior;

public interface IModuleProgressService
{
    Task<Response<IList<ModuleProgress>>> FetchAll(Request request);
    Task<ModuleProgress> Fetch(Request<Module> request);
    Task<Response> AddCompleted(Request<ModuleCompleted> request);
}
namespace Crudspa.Content.Display.Shared.Contracts.Behavior;

public interface IBinderRunService
{
    Task<Response<BinderTypeFull?>> FetchBinderType(Request<Binder> request);
    Task<Response<Binder?>> FetchBinder(Request<Binder> request);
    Task<Response<Page?>> FetchPage(Request<Page> request);
    Task<Response> AddCompleted(Request<BinderCompleted> request);
}
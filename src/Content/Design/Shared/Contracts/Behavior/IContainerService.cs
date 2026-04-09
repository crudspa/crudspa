namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IContainerService
{
    Task<Response<IList<Orderable>>> FetchDirectionNames(Request request);
    Task<Response<IList<Orderable>>> FetchWrapNames(Request request);
    Task<Response<IList<Orderable>>> FetchJustifyContentNames(Request request);
    Task<Response<IList<Orderable>>> FetchAlignItemsNames(Request request);
    Task<Response<IList<Orderable>>> FetchAlignContentNames(Request request);
}
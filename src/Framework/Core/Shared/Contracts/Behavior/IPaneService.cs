namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IPaneService
{
    Task<Response<Pane?>> Fetch(Request<Pane> request);
    Task<Response> Save(Request<Pane> request);
    Task<Response> Remove(Request<Pane> request);
    Task<Response> SaveOrder(Request<IList<Pane>> request);
    Task<Response<IList<PaneTypeFull>>> FetchPaneTypes(Request<Portal> request);
    Task<Response> Move(Request<Pane> request);
}
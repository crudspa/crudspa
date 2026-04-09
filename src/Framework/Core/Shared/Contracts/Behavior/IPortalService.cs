namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IPortalService
{
    Task<Response<IList<Portal>>> FetchAll(Request request);
    Task<Response<Portal?>> Fetch(Request<Portal> request);
    Task<Response> Save(Request<Portal> request);
}
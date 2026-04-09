namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IPortalRunService
{
    Task<Response<Portal?>> Fetch(Request<Portal> request);
}

namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IPublicationGroupService
{
    Task<Response<IList<PublicationGroup>>> FetchForPortal(Request<Portal> request);
    Task<Response<PublicationGroup?>> Fetch(Request<PublicationGroup> request);
    Task<Response<PublicationGroup?>> Add(Request<PublicationGroup> request);
    Task<Response> Save(Request<PublicationGroup> request);
    Task<Response> Remove(Request<PublicationGroup> request);
    Task<Response> SaveOrder(Request<IList<PublicationGroup>> request);
}
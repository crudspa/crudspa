namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IDistrictService
{
    Task<Response<IList<District>>> Search(Request<DistrictSearch> request);
    Task<Response<District?>> Fetch(Request<District> request);
    Task<Response<District?>> Add(Request<District> request);
    Task<Response> Save(Request<District> request);
    Task<Response> Remove(Request<District> request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request request);
}
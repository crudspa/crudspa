namespace Crudspa.Education.District.Shared.Contracts.Behavior;

using District = Data.District;

public interface IDistrictService
{
    Task<Response<District?>> Fetch(Request request);
    Task<Response> Save(Request<District> request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request request);
    Task<Response<IList<Named>>> FetchCommunityNames(Request request);
}
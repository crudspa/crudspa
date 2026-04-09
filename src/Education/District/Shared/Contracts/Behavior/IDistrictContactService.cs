namespace Crudspa.Education.District.Shared.Contracts.Behavior;

public interface IDistrictContactService
{
    Task<Response<IList<DistrictContact>>> Search(Request<DistrictContactSearch> request);
    Task<Response<DistrictContact?>> Fetch(Request<DistrictContact> request);
    Task<Response<DistrictContact?>> Add(Request<DistrictContact> request);
    Task<Response> Save(Request<DistrictContact> request);
    Task<Response> Remove(Request<DistrictContact> request);
    Task<Response<IList<Named>>> FetchRoleNames(Request request);
    Task<Response> SendAccessCode(Request<DistrictContact> request);
}
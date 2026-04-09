namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IDistrictContactService
{
    Task<Response<IList<DistrictContact>>> Search(Request<DistrictContactSearch> request);
    Task<Response<IList<DistrictContact>>> SearchForDistrict(Request<DistrictContactSearch> request);
    Task<Response<DistrictContact?>> Fetch(Request<DistrictContact> request);
    Task<Response<DistrictContact?>> Add(Request<DistrictContact> request);
    Task<Response> Save(Request<DistrictContact> request);
    Task<Response> Remove(Request<DistrictContact> request);
    Task<Response<IList<Named>>> FetchDistrictNames(Request request);
    Task<Response<IList<Named>>> FetchRoleNames(Request<District> request);
    Task<Response> SendAccessCode(Request<DistrictContact> request);
}
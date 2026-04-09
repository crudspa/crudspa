namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface ISchoolService
{
    Task<Response<IList<School>>> SearchForDistrict(Request<SchoolSearch> request);
    Task<Response<School?>> Fetch(Request<School> request);
    Task<Response<School?>> Add(Request<School> request);
    Task<Response> Save(Request<School> request);
    Task<Response> Remove(Request<School> request);
    Task<Response<IList<Named>>> FetchCommunityNames(Request request);
    Task<Response<IList<Named>>> FetchPermissionNames(Request request);
}
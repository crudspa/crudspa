namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface ISchoolContactService
{
    Task<Response<IList<SchoolContact>>> Search(Request<SchoolContactSearch> request);
    Task<Response<IList<SchoolContact>>> SearchForSchool(Request<SchoolContactSearch> request);
    Task<Response<SchoolContact?>> Fetch(Request<SchoolContact> request);
    Task<Response<SchoolContact?>> Add(Request<SchoolContact> request);
    Task<Response> Save(Request<SchoolContact> request);
    Task<Response> Remove(Request<SchoolContact> request);
    Task<Response<IList<Orderable>>> FetchTitleNames(Request request);
    Task<Response<IList<Named>>> FetchRoleNames(Request<School> request);
    Task<Response> SendAccessCode(Request<SchoolContact> request);
    Task<Response<SchoolContact>> FetchRelations(Request<SchoolContact> request);
    Task<Response> SaveRelations(Request<SchoolContact> request);
}
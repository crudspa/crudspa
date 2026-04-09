namespace Crudspa.Education.District.Shared.Contracts.Behavior;

public interface ICommunityService
{
    Task<Response<IList<Community>>> FetchAll(Request request);
    Task<Response<Community?>> Fetch(Request<Community> request);
    Task<Response<Community?>> Add(Request<Community> request);
    Task<Response> Save(Request<Community> request);
    Task<Response> Remove(Request<Community> request);
    Task<Response<IList<Selectable>>> FetchDistrictContacts(Request<Community> request);
    Task<Response<Community?>> FetchSchoolSelections(Request<Community> request);
    Task<Response> SaveSchoolSelections(Request<Community> request);
}
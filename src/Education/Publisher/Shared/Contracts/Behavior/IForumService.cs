using Forum = Crudspa.Education.Publisher.Shared.Contracts.Data.Forum;

namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IForumService
{
    Task<Response<IList<Forum>>> Search(Request<ForumSearch> request);
    Task<Response<Forum?>> Fetch(Request<Forum> request);
    Task<Response<Forum?>> Add(Request<Forum> request);
    Task<Response> Save(Request<Forum> request);
    Task<Response> Remove(Request<Forum> request);
    Task<Response<IList<Orderable>>> FetchBodyTemplateNames(Request request);
    Task<Response<IList<Named>>> FetchDistrictNames(Request request);
    Task<Response<IList<Named>>> FetchSchoolNames(Request request);
}
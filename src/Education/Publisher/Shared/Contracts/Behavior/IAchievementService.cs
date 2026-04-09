using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;

namespace Crudspa.Education.Publisher.Shared.Contracts.Behavior;

public interface IAchievementService
{
    Task<Response<IList<Achievement>>> Search(Request<AchievementSearch> request);
    Task<Response<Achievement?>> Fetch(Request<Achievement> request);
    Task<Response<Achievement?>> Add(Request<Achievement> request);
    Task<Response> Save(Request<Achievement> request);
    Task<Response> Remove(Request<Achievement> request);
    Task<Response<IList<Orderable>>> FetchRarityNames(Request request);
}
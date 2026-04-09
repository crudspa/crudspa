using Achievement = Crudspa.Education.Publisher.Shared.Contracts.Data.Achievement;
using IAchievementService = Crudspa.Education.Publisher.Shared.Contracts.Behavior.IAchievementService;

namespace Crudspa.Education.Publisher.Client.Services;

public class AchievementServiceTcp(IProxyWrappers proxyWrappers) : IAchievementService
{
    public async Task<Response<IList<Achievement>>> Search(Request<AchievementSearch> request) =>
        await proxyWrappers.Send<IList<Achievement>>("EducationAchievementSearch", request);

    public async Task<Response<Achievement?>> Fetch(Request<Achievement> request) =>
        await proxyWrappers.Send<Achievement?>("EducationAchievementFetch", request);

    public async Task<Response<Achievement?>> Add(Request<Achievement> request) =>
        await proxyWrappers.Send<Achievement?>("EducationAchievementAdd", request);

    public async Task<Response> Save(Request<Achievement> request) =>
        await proxyWrappers.Send("EducationAchievementSave", request);

    public async Task<Response> Remove(Request<Achievement> request) =>
        await proxyWrappers.Send("EducationAchievementRemove", request);

    public async Task<Response<IList<Orderable>>> FetchRarityNames(Request request) =>
        await proxyWrappers.SendAndCache<IList<Orderable>>("EducationAchievementFetchRarityNames", request);
}
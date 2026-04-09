namespace Crudspa.Education.Publisher.Client.Services;

public class CommunityServiceTcp(IProxyWrappers proxyWrappers) : ICommunityService
{
    public async Task<Response<IList<Community>>> FetchForDistrict(Request<District> request) =>
        await proxyWrappers.Send<IList<Community>>("CommunityFetchForDistrict", request);

    public async Task<Response<Community?>> Fetch(Request<Community> request) =>
        await proxyWrappers.Send<Community?>("CommunityFetch", request);

    public async Task<Response<Community?>> Add(Request<Community> request) =>
        await proxyWrappers.Send<Community?>("CommunityAdd", request);

    public async Task<Response> Save(Request<Community> request) =>
        await proxyWrappers.Send("CommunitySave", request);

    public async Task<Response> Remove(Request<Community> request) =>
        await proxyWrappers.Send("CommunityRemove", request);

    public async Task<Response<IList<Selectable>>> FetchDistrictContacts(Request<Community> request) =>
        await proxyWrappers.Send<IList<Selectable>>("CommunityFetchDistrictContacts", request);

    public async Task<Response<Community?>> FetchSchoolSelections(Request<Community> request) =>
        await proxyWrappers.Send<Community?>("CommunityFetchSchoolSelections", request);

    public async Task<Response> SaveSchoolSelections(Request<Community> request) =>
        await proxyWrappers.Send("CommunitySaveSchoolSelections", request);
}
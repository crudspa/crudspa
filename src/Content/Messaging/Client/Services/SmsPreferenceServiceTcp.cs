namespace Crudspa.Content.Messaging.Client.Services;

public class SmsPreferenceServiceTcp(IProxyWrappers proxyWrappers) : ISmsPreferenceService
{
    public async Task<Response<IList<SmsPreference>>> SearchForPortal(Request<SmsPreferenceSearch> request) =>
        await proxyWrappers.Send<IList<SmsPreference>>("SmsPreferenceSearchForPortal", request);

    public async Task<Response<SmsPreference?>> Fetch(Request<SmsPreference> request) =>
        await proxyWrappers.Send<SmsPreference?>("SmsPreferenceFetch", request);

    public async Task<Response<SmsPreference?>> Add(Request<SmsPreference> request) =>
        await proxyWrappers.Send<SmsPreference?>("SmsPreferenceAdd", request);

    public async Task<Response> Save(Request<SmsPreference> request) =>
        await proxyWrappers.Send("SmsPreferenceSave", request);

    public async Task<Response> Remove(Request<SmsPreference> request) =>
        await proxyWrappers.Send("SmsPreferenceRemove", request);

    public async Task<Response<IList<Named>>> FetchContactNames(Request request) =>
        await proxyWrappers.Send<IList<Named>>("SmsPreferenceFetchContactNames", request);

    public async Task<Response<IList<Orderable>>> FetchContactPhoneNames(Request request) =>
        await proxyWrappers.Send<IList<Orderable>>("SmsPreferenceFetchContactPhoneNames", request);
}
namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface ISmsPreferenceService
{
    Task<Response<IList<SmsPreference>>> SearchForPortal(Request<SmsPreferenceSearch> request);
    Task<Response<SmsPreference?>> Fetch(Request<SmsPreference> request);
    Task<Response<SmsPreference?>> Add(Request<SmsPreference> request);
    Task<Response> Save(Request<SmsPreference> request);
    Task<Response> Remove(Request<SmsPreference> request);
    Task<Response<IList<Named>>> FetchContactNames(Request request);
    Task<Response<IList<Orderable>>> FetchContactPhoneNames(Request request);
}
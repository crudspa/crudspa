namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface ISmsMessageService
{
    Task<Response<IList<SmsMessage>>> SearchForPortal(Request<SmsMessageSearch> request);
    Task<Response<IList<SmsMessage>>> SearchForMembership(Request<SmsMessageSearch> request);
    Task<Response<IList<SmsMessage>>> SearchForContactPhone(Request<SmsMessageSearch> request);
    Task<Response<IList<SmsMessage>>> SearchForContact(Request<SmsMessageSearch> request);
    Task<Response<IList<SmsMessage>>> SearchThread(Request<SmsMessage> request);
    Task<Response<SmsMessage?>> Fetch(Request<SmsMessage> request);
    Task<Response<SmsMessage?>> Reply(Request<SmsMessage> request);
}
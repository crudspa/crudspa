namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface ISmsMessageMediaService
{
    Task<Response<IList<SmsMessageMedia>>> FetchForSmsMessage(Request<SmsMessage> request);
    Task<Response> SaveOrder(Request<IList<SmsMessageMedia>> request);
}
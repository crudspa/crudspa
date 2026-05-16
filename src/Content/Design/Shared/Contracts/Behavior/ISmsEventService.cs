namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface ISmsEventService
{
    Task<Response<IList<SmsEvent>>> Search(Request<SmsEventSearch> request);
    Task<Response<IList<SmsEvent>>> SearchForSmsMessage(Request<SmsEventSearch> request);
}
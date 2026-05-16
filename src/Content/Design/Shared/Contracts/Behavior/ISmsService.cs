namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface ISmsService
{
    Task<Response<IList<Sms>>> SearchForMembership(Request<SmsSearch> request);
    Task<Response<IList<Sms>>> SearchForPortal(Request<SmsSearch> request);
    Task<Response<Sms?>> Fetch(Request<Sms> request);
    Task<Response<Sms?>> Add(Request<Sms> request);
    Task<Response> Save(Request<Sms> request);
    Task<Response> Remove(Request<Sms> request);
    Task<Response<IList<SmsTemplateFull>>> FetchSmsTemplates(Request request);
}
namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface ISmsTemplateService
{
    Task<Response<IList<SmsTemplate>>> SearchForMembership(Request<SmsTemplateSearch> request);
    Task<Response<IList<SmsTemplate>>> SearchForPortal(Request<SmsTemplateSearch> request);
    Task<Response<SmsTemplate?>> Fetch(Request<SmsTemplate> request);
    Task<Response<SmsTemplate?>> Add(Request<SmsTemplate> request);
    Task<Response> Save(Request<SmsTemplate> request);
    Task<Response> Remove(Request<SmsTemplate> request);
}
namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IEmailTemplateService
{
    Task<Response<IList<EmailTemplate>>> SearchForMembership(Request<EmailTemplateSearch> request);
    Task<Response<EmailTemplate?>> Fetch(Request<EmailTemplate> request);
    Task<Response<EmailTemplate?>> Add(Request<EmailTemplate> request);
    Task<Response> Save(Request<EmailTemplate> request);
    Task<Response> Remove(Request<EmailTemplate> request);
    Task<Response<IList<Token>>> FetchTokens(Request<Membership> request);
}
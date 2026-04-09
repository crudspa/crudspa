using Membership = Crudspa.Content.Design.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Design.Shared.Contracts.Behavior;

public interface IEmailService
{
    Task<Response<IList<Email>>> SearchForMembership(Request<EmailSearch> request);
    Task<Response<Email?>> Fetch(Request<Email> request);
    Task<Response<Email?>> Add(Request<Email> request);
    Task<Response> Save(Request<Email> request);
    Task<Response> Remove(Request<Email> request);
    Task<Response<IList<EmailTemplateFull>>> FetchEmailTemplates(Request<Membership> request);
    Task<Response<IList<Token>>> FetchTokens(Request<Membership> request);
}
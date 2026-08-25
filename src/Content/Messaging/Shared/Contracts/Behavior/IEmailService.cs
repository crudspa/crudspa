using Membership = Crudspa.Content.Messaging.Shared.Contracts.Data.Membership;

namespace Crudspa.Content.Messaging.Shared.Contracts.Behavior;

public interface IEmailService
{
    Task<Response<IList<Email>>> SearchForMembership(Request<EmailSearch> request);
    Task<Response<IList<Email>>> SearchForPortal(Request<EmailSearch> request);
    Task<Response<IList<EmailSent>>> SearchSentForEmail(Request<EmailSentSearch> request);
    Task<Response<Email?>> Fetch(Request<Email> request);
    Task<Response<IList<Named>>> FetchMembershipNamesForActivation(Request<Activation> request);
    Task<Response<IList<Named>>> FetchStageNamesForActivation(Request<Activation> request);
    Task<Response<Email?>> Add(Request<Email> request);
    Task<Response> Save(Request<Email> request);
    Task<Response> Remove(Request<Email> request);
    Task<Response<IList<EmailTemplateFull>>> FetchEmailTemplates(Request<Membership> request);
    Task<Response<IList<Token>>> FetchTokens(Request<Membership> request);
}
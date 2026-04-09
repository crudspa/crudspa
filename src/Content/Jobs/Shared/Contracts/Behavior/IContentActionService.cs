namespace Crudspa.Content.Jobs.Shared.Contracts.Behavior;

public interface IContentActionService
{
    Task<Response<IList<Email>>> FetchEmailsForSending(Request request);
    Task<Response<IList<Member>>> FetchMembers(Request<Membership> request);
    Task<Response> SaveLog(Request<EmailLog> request);
    Task<Response> UpdateStatus(Request<Email> request);
}
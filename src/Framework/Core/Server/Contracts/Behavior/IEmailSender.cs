namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface IEmailSender
{
    Task<Response> Send(Request<EmailMessage> request);
}
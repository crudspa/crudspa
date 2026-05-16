namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISmsSender
{
    Task<Response> Send(Request<SmsOutboundMessage> request);
}
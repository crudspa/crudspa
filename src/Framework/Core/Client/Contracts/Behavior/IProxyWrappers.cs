namespace Crudspa.Framework.Core.Client.Contracts.Behavior;

public interface IProxyWrappers
{
    Guid? SessionId { get; }
    Task SetSessionId(Guid? sessionId);
    Task Log(ClientLogEntry entry);
    Task<Response> Send(String methodName, Request request);
    Task<Response<T>> Send<T>(String methodName, Request request) where T : class?;
    Task<Response<T>> SendAndCache<T>(String methodName, Request request) where T : class?;
}
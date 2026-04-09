namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISessionService
{
    Task<Response<Session?>> FetchOrCreate(Request<Session> request);
    Task<Response> IsValidForSignIn(Request<Session> request);
    Task InvalidateAll();
    Task<Response> End(Request request);
}
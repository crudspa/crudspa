namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface ILinkClickService
{
    Task<Response> Add(Request<LinkClick> request);
}
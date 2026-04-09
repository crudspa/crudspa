namespace Crudspa.Framework.Core.Shared.Contracts.Behavior;

public interface IMediaPlayService
{
    Task<Response> Add(Request<MediaPlay> request);
}
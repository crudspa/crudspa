namespace Crudspa.Framework.Core.Server.Contracts.Behavior;

public interface ISegmentFetcher
{
    Task<IList<NavSegment>> Fetch(Guid? sessionId, Guid? portalId);
}
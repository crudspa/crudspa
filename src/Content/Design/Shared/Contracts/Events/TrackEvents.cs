namespace Crudspa.Content.Design.Shared.Contracts.Events;

public class TrackPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
}

public class TrackAdded : TrackPayload;

public class TrackSaved : TrackPayload;

public class TrackRemoved : TrackPayload;

public class TracksReordered : TrackPayload;
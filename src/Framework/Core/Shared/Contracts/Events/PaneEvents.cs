namespace Crudspa.Framework.Core.Shared.Contracts.Events;

public class PanePayload
{
    public Guid? Id { get; set; }
    public Guid? SegmentId { get; set; }
}

public class PaneAdded : PanePayload;

public class PaneSaved : PanePayload;

public class PaneRemoved : PanePayload;

public class PanesReordered : PanePayload;

public class PaneMoved : PanePayload
{
    public Guid? OldSegmentId { get; set; }
    public Guid? NewSegmentId { get; set; }
}
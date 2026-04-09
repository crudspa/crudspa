namespace Crudspa.Framework.Core.Shared.Contracts.Events;

public class SegmentPayload
{
    public Guid? Id { get; set; }
    public Guid? PortalId { get; set; }
    public Guid? ParentId { get; set; }
}

public class SegmentAdded : SegmentPayload;

public class SegmentSaved : SegmentPayload;

public class SegmentRemoved : SegmentPayload;

public class SegmentsReordered : SegmentPayload;

public class SegmentMoved : SegmentPayload
{
    public Guid? OldPortalId { get; set; }
    public Guid? OldParentId { get; set; }
    public Guid? NewPortalId { get; set; }
    public Guid? NewParentId { get; set; }
}
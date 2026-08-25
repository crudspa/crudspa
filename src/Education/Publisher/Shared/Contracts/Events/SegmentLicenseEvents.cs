namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class SegmentLicensePayload
{
    public Guid? Id { get; set; }
    public Guid? LicenseId { get; set; }
}

public class SegmentLicenseAdded : SegmentLicensePayload;

public class SegmentLicenseSaved : SegmentLicensePayload;

public class SegmentLicenseRemoved : SegmentLicensePayload;
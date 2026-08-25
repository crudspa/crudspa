namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class TrackLicensePayload
{
    public Guid? Id { get; set; }
    public Guid? LicenseId { get; set; }
}

public class TrackLicenseAdded : TrackLicensePayload;

public class TrackLicenseSaved : TrackLicensePayload;

public class TrackLicenseRemoved : TrackLicensePayload;
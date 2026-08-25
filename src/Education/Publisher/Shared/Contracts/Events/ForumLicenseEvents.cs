namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class ForumLicensePayload
{
    public Guid? Id { get; set; }
    public Guid? LicenseId { get; set; }
}

public class ForumLicenseAdded : ForumLicensePayload;

public class ForumLicenseSaved : ForumLicensePayload;

public class ForumLicenseRemoved : ForumLicensePayload;
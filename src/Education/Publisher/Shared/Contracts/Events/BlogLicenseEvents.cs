namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class BlogLicensePayload
{
    public Guid? Id { get; set; }
    public Guid? LicenseId { get; set; }
}

public class BlogLicenseAdded : BlogLicensePayload;

public class BlogLicenseSaved : BlogLicensePayload;

public class BlogLicenseRemoved : BlogLicensePayload;
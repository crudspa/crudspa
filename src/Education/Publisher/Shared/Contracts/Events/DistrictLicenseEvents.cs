namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class DistrictLicensePayload
{
    public Guid? Id { get; set; }
    public Guid? LicenseId { get; set; }
}

public class DistrictLicenseAdded : DistrictLicensePayload;

public class DistrictLicenseSaved : DistrictLicensePayload;

public class DistrictLicenseRemoved : DistrictLicensePayload;

public class DistrictLicenseRelationsSaved : DistrictLicensePayload;
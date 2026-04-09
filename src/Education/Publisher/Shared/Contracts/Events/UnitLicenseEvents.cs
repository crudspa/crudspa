namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class UnitLicensePayload
{
    public Guid? Id { get; set; }
    public Guid? LicenseId { get; set; }
}

public class UnitLicenseAdded : UnitLicensePayload;

public class UnitLicenseSaved : UnitLicensePayload;

public class UnitLicenseRemoved : UnitLicensePayload;

public class UnitLicenseRelationsSaved : UnitLicensePayload;
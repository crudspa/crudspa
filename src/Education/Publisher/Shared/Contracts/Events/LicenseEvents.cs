namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class LicensePayload
{
    public Guid? Id { get; set; }
}

public class LicenseAdded : LicensePayload;

public class LicenseSaved : LicensePayload;

public class LicenseRemoved : LicensePayload;
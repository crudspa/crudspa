namespace Crudspa.Framework.Core.Shared.Contracts.Events;

public class LicensePayload
{
    public Guid? Id { get; set; }
}

public class LicenseAdded : LicensePayload;

public class LicenseSaved : LicensePayload;

public class LicenseRemoved : LicensePayload;

public class LicenseRelationsSaved : LicensePayload;
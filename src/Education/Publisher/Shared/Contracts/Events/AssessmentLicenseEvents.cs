namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class AssessmentLicensePayload
{
    public Guid? Id { get; set; }
    public Guid? LicenseId { get; set; }
}

public class AssessmentLicenseAdded : AssessmentLicensePayload;

public class AssessmentLicenseSaved : AssessmentLicensePayload;

public class AssessmentLicenseRemoved : AssessmentLicensePayload;
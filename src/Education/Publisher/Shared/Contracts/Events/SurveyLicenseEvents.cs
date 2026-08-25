namespace Crudspa.Education.Publisher.Shared.Contracts.Events;

public class SurveyLicensePayload
{
    public Guid? Id { get; set; }
    public Guid? LicenseId { get; set; }
}

public class SurveyLicenseAdded : SurveyLicensePayload;

public class SurveyLicenseSaved : SurveyLicensePayload;

public class SurveyLicenseRemoved : SurveyLicensePayload;
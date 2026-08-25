namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class CampaignLicense : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? LicenseId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CampaignId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CampaignName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!CampaignId.HasValue)
                errors.AddError("Campaign is required.", nameof(CampaignId));
        });
    }
}
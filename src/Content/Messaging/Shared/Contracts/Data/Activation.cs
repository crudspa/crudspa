namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class Activation : Observable, IValidates, INamed, ICountable
{
    public String? Name => CampaignName;

    public String? CampaignName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BatchId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? OrganizationId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? OrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CampaignId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? Start
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Activated
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivatedBy
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
        });
    }
}
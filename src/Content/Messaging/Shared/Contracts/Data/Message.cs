namespace Crudspa.Content.Messaging.Shared.Contracts.Data;

public class Message : Observable, IValidates, INamed, ICountable
{
    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? MembershipId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PopulationId { get; set => SetProperty(ref field, value); }
    public Guid? MessageTypeId { get; set => SetProperty(ref field, value); }
    public Guid? EmailTemplateId { get; set => SetProperty(ref field, value); }
    public Guid? SmsTemplateId { get; set => SetProperty(ref field, value); }
    public Int32? Ordinal { get; set => SetProperty(ref field, value); }
    public Guid? DefinitionId { get; set => SetProperty(ref field, value); }

    public String? StageName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivationId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? EmailId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SmsId
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
            if (ActivationId is null)
            {
                if (!StageId.HasValue) errors.AddError("Stage is required.", nameof(StageId));
                if (Name.HasNothing()) errors.AddError("Name is required.", nameof(Name));
                if (!PopulationId.HasValue) errors.AddError("Population is required.", nameof(PopulationId));
                if (!MessageTypeId.HasValue) errors.AddError("Channel is required.", nameof(MessageTypeId));
                if (EmailTemplateId.HasValue == SmsTemplateId.HasValue) errors.AddError("Choose exactly one Email or SMS template.", nameof(EmailTemplateId));
            }
        });
    }
}
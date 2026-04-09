namespace Crudspa.Framework.Jobs.Shared.Contracts.Data;

public class Job : Observable, IValidates, ICountable, INamed
{
    public String Name => TypeName + " | " + Description;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Config
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Added
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Started
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Ended
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? DeviceId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ScheduleId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BatchId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public JobTypeFull? Type
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusCssClass
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DeviceName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ScheduleName
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
            if (!Id.HasValue)
                errors.AddError("An ID must be generated externally for the job.", nameof(Id));

            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));

            if (Config.HasNothing())
                errors.AddError("Config is required.", nameof(Config));

            if (Description.HasNothing())
                errors.AddError("Description is required.", nameof(Description));
        });
    }
}
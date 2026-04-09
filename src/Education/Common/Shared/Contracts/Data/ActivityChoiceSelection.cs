namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ActivityChoiceSelection : Observable, IUnique, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssignmentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ChoiceId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Made
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TargetChoiceId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ChoiceText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!AssignmentId.HasValue)
                errors.AddError("Assignment is required.", nameof(AssignmentId));

            if (!ChoiceId.HasValue)
                errors.AddError("Choice is required.", nameof(ChoiceId));

            if (!Made.HasValue)
                errors.AddError("Made is required.", nameof(Made));
        });
    }
}
namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class Forum : Observable, IValidates, INamed, ICountable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BodyTemplateId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Pinned
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? DistrictId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SchoolId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? InnovatorsOnly
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BodyTemplateName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DistrictStudentIdNumberLabel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SchoolKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PostCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? DistrictName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SchoolName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 75)
                errors.AddError("Name cannot be longer than 75 characters.", nameof(Name));

            if (Description.HasNothing())
                errors.AddError("Description is required.", nameof(Description));

            if (!BodyTemplateId.HasValue)
                errors.AddError("Body Template is required.", nameof(BodyTemplateId));

            if (!InnovatorsOnly.HasValue)
                errors.AddError("Innovators Only is required.", nameof(InnovatorsOnly));
        });
    }
}
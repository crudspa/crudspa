namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class District : Observable, IValidates, INamed, ICountable
{
    public String? Name => Organization.Name;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StudentIdNumberLabel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentExplainer
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? OrganizationId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AddressId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Organization Organization
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public UsaPostal UsaPostal
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? DistrictContactCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? CommunityCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SchoolCount
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
            if (StudentIdNumberLabel.HasNothing())
                errors.AddError("Student Id Number Label is required.", nameof(StudentIdNumberLabel));
            else if (StudentIdNumberLabel!.Length > 50)
                errors.AddError("Student Id Number Label cannot be longer than 50 characters.", nameof(StudentIdNumberLabel));

            if (AssessmentExplainer.HasNothing())
                errors.AddError("Assessment Explainer is required.", nameof(AssessmentExplainer));

            errors.AddRange(Organization.Validate());
            errors.AddRange(UsaPostal.Validate());
        });
    }
}
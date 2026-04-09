namespace Crudspa.Education.School.Shared.Contracts.Data;

public class District : Observable
{
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

    public String? OrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }
}
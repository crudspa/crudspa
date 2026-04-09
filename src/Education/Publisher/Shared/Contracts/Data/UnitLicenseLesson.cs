namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class UnitLicenseLesson : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UnitLicenseId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? LessonId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? LessonTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? LessonOrdinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Selected
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors => { });
    }
}
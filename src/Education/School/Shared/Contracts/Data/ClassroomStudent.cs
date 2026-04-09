namespace Crudspa.Education.School.Shared.Contracts.Data;

public class ClassroomStudent : Observable, IUnique, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ClassroomId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StudentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SecretCode
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? IsTestAccount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!ClassroomId.HasValue)
                errors.AddError("Classroom is required.", nameof(ClassroomId));

            if (!StudentId.HasValue)
                errors.AddError("Student is required.", nameof(Student));
        });
    }
}
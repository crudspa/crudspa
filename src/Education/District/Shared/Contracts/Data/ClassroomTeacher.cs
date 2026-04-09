namespace Crudspa.Education.District.Shared.Contracts.Data;

public class ClassroomTeacher : Observable, IUnique, IValidates
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

    public Guid? SchoolContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public SchoolContact? SchoolContact
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

            if (!SchoolContactId.HasValue)
                errors.AddError("School Contact is required.", nameof(SchoolContactId));
        });
    }
}
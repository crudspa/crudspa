namespace Crudspa.Education.District.Shared.Contracts.Data;

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

    public Classroom? Classroom
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Student? Student
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StudentImportResearchId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ClassroomImportNum
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
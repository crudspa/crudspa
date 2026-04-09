namespace Crudspa.Education.School.Shared.Contracts.Data;

public class Classroom : Observable, IValidates, INamed
{
    private String? _organizationName;

    public String? Name => _organizationName;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SchoolId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SchoolYearId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GradeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ImportNum
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public String? OrganizationName
    {
        get => _organizationName;
        set => SetProperty(ref _organizationName, value);
    }

    public Int32? SchoolImportNum
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Int32? ClassroomStudentCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ClassroomTeacherCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ClassroomStudent> ClassroomStudents
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ClassroomTeacher> ClassroomTeachers
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> Students
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> SchoolContacts
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GradeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? SmallClassroom
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (OrganizationName.HasNothing())
                errors.AddError("Name is required.", nameof(OrganizationName));
            else if (OrganizationName!.Length > 75)
                errors.AddError("Name cannot be longer than 75 characters.", nameof(OrganizationName));
            if (!SmallClassroom.HasValue)
                errors.AddError("Small Classroom is required.", nameof(SmallClassroom));
        });
    }
}
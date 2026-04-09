namespace Crudspa.Education.District.Shared.Contracts.Data;

public class Classroom : Observable, IUnique, IValidates
{
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

    public Int32? ImportNum
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public String? OrganizationName
    {
        get;
        set => SetProperty(ref field, value);
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

    public Guid? SchoolDistrictId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SchoolDistrictOrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SchoolOrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SchoolYearName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? SmallClassroom
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Guid? GradeId
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
            if (String.IsNullOrWhiteSpace(OrganizationName))
                errors.AddError("Name is required.", nameof(OrganizationName));
            else if (OrganizationName.Length > 75)
                errors.AddError("Name cannot be longer than 75 characters.", nameof(OrganizationName));

            if (!SchoolId.HasValue)
                errors.AddError("School is required.", nameof(SchoolId));

            if (!SchoolYearId.HasValue)
                errors.AddError("School Year is required.", nameof(SchoolYearId));

            ClassroomStudents.Apply(x => errors.AddRange(x.Validate()));
            ClassroomTeachers.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}
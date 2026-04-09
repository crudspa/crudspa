namespace Crudspa.Education.District.Shared.Contracts.Data;

public class Student : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? MaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FirstName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? LastName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SecretCode
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GradeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? IsTestAccount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable> SchoolYears
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FriendlyAssessmentLevel
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PreferredName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AvatarString
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? IdNumber
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? FamilySchoolId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SchoolName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ClassroomStudent> ClassroomStudents
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (FirstName.HasSomething() && FirstName!.Length > 75)
                errors.AddError("First Name cannot be longer than 75 characters.", nameof(FirstName));

            if (LastName.HasSomething() && LastName!.Length > 75)
                errors.AddError("Last Name cannot be longer than 75 characters.", nameof(LastName));

            if (!IdNumber.HasSomething())
                errors.AddError("ID Number is required.", nameof(IdNumber));

            if (String.IsNullOrWhiteSpace(SecretCode))
                errors.AddError("Secret Code is required.", nameof(SecretCode));
            else if (SecretCode.Length > 75)
                errors.AddError("Secret Code cannot be longer than 75 characters.", nameof(SecretCode));

            if (PreferredName.HasSomething() && PreferredName!.Length > 75)
                errors.AddError("Preferred Name cannot be longer than 75 characters.", nameof(PreferredName));

            if (IdNumber.HasSomething() && IdNumber!.Length > 35)
                errors.AddError("Id Number cannot be longer than 35 characters.", nameof(IdNumber));
        });
    }
}
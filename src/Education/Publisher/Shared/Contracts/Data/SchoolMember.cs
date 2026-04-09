namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class SchoolMember : Contact, IRelates
{
    public Guid? UserId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SchoolId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TitleId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TitleName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? TestAccount
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? Treatment
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? MaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UserOrganizationId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? UserUsername
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? UserResetPassword
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Int32? AddressCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? ResetPassword
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? IdNumber
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable> Roles
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> SchoolYears
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<RelatedEntity> Relations
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

    public Int32? SchoolContactsThatMaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SchoolContactsThatHaveSignedIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? SchoolContactSignInPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StudentsThatMaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StudentsThatHaveSignedIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? StudentSignInPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? DistrictStudentsThatMaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? DistrictStudentsThatHaveSignedIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? DistrictStudentSignInPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ClassroomStudentsThatMaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ClassroomStudentsThatHaveSignedIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? ClassroomStudentSignInPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PreviousSchoolContactsThatMaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PreviousSchoolContactsThatHaveSignedIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? PreviousSchoolContactSignInPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PreviousStudentsThatMaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PreviousStudentsThatHaveSignedIn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? PreviousStudentSignInPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SchoolGamesCompleted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StudentsThatCouldCompleteGame
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StudentsThatDidCompleteGame
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? StudentCompleteGamePercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? DistrictStudentsThatCouldCompleteGame
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? DistrictStudentsThatDidCompleteGame
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? DistrictStudentCompleteGamePercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ClassroomStudentsThatCouldCompleteGame
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ClassroomStudentsThatDidCompleteGame
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? ClassroomStudentCompleteGamePercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? ClassroomStudentAssessmentPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? DistrictStudentAssessmentPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? StudentAssessmentPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SchoolAssessmentsCompleted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StudentsThatCouldCompleteAssessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StudentsThatDidCompleteAssessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PreviousStudentsThatCouldCompleteAssessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PreviousStudentsThatDidCompleteAssessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? PreviousStudentAssessmentPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ClassroomStudentsThatCouldCompleteAssessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ClassroomStudentsThatDidCompleteAssessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? DistrictStudentsThatCouldCompleteAssessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? DistrictStudentsThatDidCompleteAssessment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SenderFirstName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SenderLastName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MoreTeacherLeaderName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? MORE101Percentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? More101LessonsPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? More101DigitalPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? More101TransferPercentage
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? LessonCompletedPercentageClassroom
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? LessonCompletedPercentageSchool
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Decimal? SchoolContactSignInPercentage1stGrade { get; set; }
    public Decimal? SchoolContactSignInPercentage2ndGrade { get; set; }
    public Decimal? SchoolContactSignInPercentage3rdGrade { get; set; }
    public Decimal? SchoolContactSignInPercentage4thGrade { get; set; }

    public Decimal? StudentSignInPercentage1stGrade { get; set; }
    public Decimal? StudentSignInPercentage2ndGrade { get; set; }
    public Decimal? StudentSignInPercentage3rdGrade { get; set; }
    public Decimal? StudentSignInPercentage4thGrade { get; set; }

    public Decimal? StudentCompleteGamePercentage1stGrade { get; set; }
    public Decimal? StudentCompleteGamePercentage2ndGrade { get; set; }
    public Decimal? StudentCompleteGamePercentage3rdGrade { get; set; }
    public Decimal? StudentCompleteGamePercentage4thGrade { get; set; }

    public Decimal? More101DigitalPercentage1stGrade { get; set; }
    public Decimal? More101DigitalPercentage2ndGrade { get; set; }
    public Decimal? More101DigitalPercentage3rdGrade { get; set; }
    public Decimal? More101DigitalPercentage4thGrade { get; set; }

    public override List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (FirstName.HasNothing())
                errors.AddError("First Name is required.", nameof(FirstName));
            else if (FirstName!.Length > 75)
                errors.AddError("First Name cannot be longer than 75 characters.", nameof(FirstName));

            if (LastName.HasNothing())
                errors.AddError("Last Name is required.", nameof(LastName));
            else if (LastName!.Length > 75)
                errors.AddError("Last Name cannot be longer than 75 characters.", nameof(LastName));

            if (TimeZoneId.HasNothing())
                errors.AddError("Time Zone is required.", nameof(TimeZoneId));
            else if (TimeZoneId!.Length > 32)
                errors.AddError("Time Zone cannot be longer than 32 characters.", nameof(TimeZoneId));

            if (MaySignIn == true)
            {
                if (UserUsername.HasNothing())
                    errors.AddError("Username is required.", nameof(UserUsername));
                else if (UserUsername!.Length > 75)
                    errors.AddError("Username cannot be longer than 75 characters.", nameof(UserUsername));
            }

            if (!TitleId.HasValue)
                errors.AddError("Title is required.", nameof(TitleId));

            if (!TestAccount.HasValue)
                errors.AddError("Test Account is required.", nameof(TestAccount));
        });
    }
}
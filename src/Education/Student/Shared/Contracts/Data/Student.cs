namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Student : Observable, IUnique
{
    public Guid? Id
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

    public String? TimeZoneId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContactId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UserId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ResearchId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? FamilyId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SecretCode
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GenderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GradeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentTypeGroupId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentLevelGroupId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ConditionGroupId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GoalSettingGroupId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PersonalizationGroupId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContentGroupId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AudioGenderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ChallengeLevelId
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

    public DateTimeOffset? TermsAccepted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? IsTestAccount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ResearchGroupId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AudioGenderName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ChallengeLevelName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? FamilySchoolId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FamilySchoolKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FamilySchoolOrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FamilySchoolDistrictOrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GradeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? StudentBookCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ActivityAssignment>? ActivityAssignments
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<StudentAssessment>? StudentAssessments
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ClassroomId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BaseUri
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SchoolImportNum
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? FamilyImportNum
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable>? Books
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable>? SchoolYears
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Email
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? MaySignIn
    {
        get;
        set => SetProperty(ref field, value);
    }
}
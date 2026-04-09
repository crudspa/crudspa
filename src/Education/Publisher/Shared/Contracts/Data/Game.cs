namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class Game : Observable, IValidates, ICountable, INamed
{
    private String? _key;

    public String? Name => _key;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BookKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Key
    {
        get => _key;
        set => SetProperty(ref _key, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? IconId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? IconName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GradeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GradeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentLevelId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentLevelKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? RequiresAchievementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RequiresAchievementTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GeneratesAchievementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GeneratesAchievementTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? GameSectionCount
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
            if (Key.HasNothing())
                errors.AddError("Key is required.", nameof(Key));
            else if (Key!.Length > 75)
                errors.AddError("Key cannot be longer than 75 characters.", nameof(Key));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!GradeId.HasValue)
                errors.AddError("Grade is required.", nameof(GradeId));

            if (!AssessmentLevelId.HasValue)
                errors.AddError("Assessment Level is required.", nameof(AssessmentLevelId));

            if (!IconId.HasValue)
                errors.AddError("Icon is required.", nameof(GradeId));
        });
    }
}
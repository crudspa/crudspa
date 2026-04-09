namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Game : Observable, IUnique, IValidates
{
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

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? IconName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GradeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentLevelId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? RequiresAchievementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GeneratesAchievementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentLevelName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GeneratesAchievementTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GradeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RequiresAchievementTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? GameSectionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<GameActivity> GameActivities
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Achievement RequiresAchievement
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Achievement GeneratesAchievement
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? GameRunId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public GameProgress Progress
    {
        get;
        set => SetProperty(ref field, value);
    } = new() { GameCompletedCount = 0 };

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!BookId.HasValue)
                errors.AddError("Book is required.", nameof(BookId));

            if (String.IsNullOrWhiteSpace(Key))
                errors.AddError("Key is required.", nameof(Key));
            else if (Key.Length > 75)
                errors.AddError("Key cannot be longer than 75 characters.", nameof(Key));

            if (String.IsNullOrWhiteSpace(Title))
                errors.AddError("Title is required.", nameof(Title));
            else if (Title.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (String.IsNullOrWhiteSpace(IconName))
                errors.AddError("Icon Name is required.", nameof(IconName));
            else if (IconName.Length > 75)
                errors.AddError("Icon Name cannot be longer than 75 characters.", nameof(IconName));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!GradeId.HasValue)
                errors.AddError("Grade is required.", nameof(GradeId));

            if (!AssessmentLevelId.HasValue)
                errors.AddError("Assessment Level is required.", nameof(AssessmentLevelId));
        });
    }
}
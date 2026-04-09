namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Objective : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
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

    public Guid? LessonId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TrophyImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BinderId
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

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public String? LessonTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? LessonUnitId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile LessonGuideImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? LessonUnitTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile TrophyImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? BinderDisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Achievement? RequiresAchievement
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Achievement? GeneratesAchievement
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (String.IsNullOrWhiteSpace(Title))
                errors.AddError("Title is required.", nameof(Title));
            else if (Title.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!LessonId.HasValue)
                errors.AddError("Lesson is required.", nameof(LessonId));

            if (!Ordinal.HasValue)
                errors.AddError("Ordinal is required.", nameof(Ordinal));
        });
    }
}
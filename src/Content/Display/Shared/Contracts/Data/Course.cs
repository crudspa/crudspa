namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Course : Observable, IValidates, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TrackId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TrackTitle
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

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Description
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

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Binder Binder
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Achievement? GeneratesAchievement
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Achievement? RequiresAchievement
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TrackDescription
    {
        get;
        set => SetProperty(ref field, value);
    }

    public CourseProgress Progress
    {
        get;
        set => SetProperty(ref field, value);
    } = new() { TimesCompleted = 0 };

    public Boolean Locked
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PageCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (Description.HasNothing())
                errors.AddError("Description is required.", nameof(Description));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            errors.AddRange(Binder.Validate());
        });
    }
}
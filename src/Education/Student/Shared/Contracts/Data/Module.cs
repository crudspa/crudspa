namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Module : Observable, IUnique, IValidates
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

    public String? IconName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
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

    public String? BookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile BookGuideImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? GeneratesAchievementTitle
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

    public Int32? PageCount
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public String? BinderDisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

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

    public ModuleProgress Progress
    {
        get;
        set => SetProperty(ref field, value);
    } = new() { ModuleCompletedCount = 0 };

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (String.IsNullOrWhiteSpace(Title))
                errors.AddError("Title is required.", nameof(Title));
            else if (Title.Length > 75)
                errors.AddError("Title cannot be longer than 75 characters.", nameof(Title));

            if (String.IsNullOrWhiteSpace(IconName))
                errors.AddError("Icon Name is required.", nameof(IconName));
            else if (IconName.Length > 75)
                errors.AddError("Icon Name cannot be longer than 75 characters.", nameof(IconName));

            if (!BookId.HasValue)
                errors.AddError("Book is required.", nameof(BookId));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!Ordinal.HasValue)
                errors.AddError("Ordinal is required.", nameof(Ordinal));
        });
    }
}
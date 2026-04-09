namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class Lesson : Observable, IValidates, IOrderable, INamed
{
    public String? Name => Title;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? UnitTitle
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

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile GuideImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? GuideText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile GuideAudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

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

    public Boolean? RequireSequentialCompletion
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Boolean? Treatment
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Boolean? Control
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

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

    public Int32? ObjectiveCount
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

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (ImageFile.Name.HasNothing())
                errors.AddError("Image is required.", nameof(ImageFile));

            if (!RequireSequentialCompletion.HasValue)
                errors.AddError("Require Sequential Completion is required.", nameof(RequireSequentialCompletion));

            if (!Treatment.HasValue)
                errors.AddError("Treatment is required.", nameof(Treatment));

            if (!Control.HasValue)
                errors.AddError("Control is required.", nameof(Control));
        });
    }
}
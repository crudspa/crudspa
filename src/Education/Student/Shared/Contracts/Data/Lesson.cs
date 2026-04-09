namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Lesson : Observable, IUnique, IValidates
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

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GuideImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GuideText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GuideAudioId
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

    public Boolean? RequireSequentialCompletion
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Treatment
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? Control
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public AudioFile GuideAudio
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile GuideImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile Image
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? UnitTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ObjectiveLite> Objectives
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

            if (!UnitId.HasValue)
                errors.AddError("Unit is required.", nameof(UnitId));

            if (!Ordinal.HasValue)
                errors.AddError("Ordinal is required.", nameof(Ordinal));

            errors.AddRange(Image.Validate());
        });
    }
}
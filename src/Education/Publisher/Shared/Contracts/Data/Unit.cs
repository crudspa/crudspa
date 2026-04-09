namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class Unit : Observable, IValidates, IOrderable
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

    public String? StatusName
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

    public Guid? ParentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ParentTitle
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

    public ImageFile ImageFile
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

    public AudioFile IntroAudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public AudioFile SongAudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean? Treatment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Control
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? LessonCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? UnitBookCount
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

            if (!GradeId.HasValue)
                errors.AddError("Grade is required.", nameof(GradeId));

            if (ImageFile.Name.HasNothing())
                errors.AddError("Image is required.", nameof(ImageFile));

            if (!Treatment.HasValue)
                errors.AddError("Treatment is required.", nameof(Treatment));

            if (!Control.HasValue)
                errors.AddError("Control is required.", nameof(Control));

            if (ParentId is not null && ParentId.Equals(Id))
                errors.AddError("Parent field cannot point to itself.", nameof(ParentId));
        });
    }
}
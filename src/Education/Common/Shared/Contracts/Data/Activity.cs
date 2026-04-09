namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class Activity : Observable, IUnique, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ActivityTypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContentAreaId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContextText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContextAudioFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ExtraText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ContextImageFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityTypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityTypeKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityTypeDisplayView
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityTypeCategoryKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ActivityTypeCategoryName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? ActivityTypeShuffleChoices
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContentAreaName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContentAreaKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ContentAreaAppNavText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile ContextAudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile ContextImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ActivityChoice> ActivityChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ActivityAssignment Assignment
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!ActivityTypeId.HasValue)
                errors.AddError("Activity Type is required.", nameof(ActivityTypeId));

            if (!ContentAreaId.HasValue)
                errors.AddError("Content Area is required.", nameof(ContentAreaId));

            if (ContextAudioFile.BlobId is not null)
                errors.AddRange(ContextAudioFile.Validate());

            if (ContextImageFile.BlobId is not null)
                errors.AddRange(ContextImageFile.Validate());

            ActivityChoices.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}
namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Page : Observable, IValidates, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
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

    public Boolean? ShowNotebook
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? ShowGuide
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

    public AudioFile GuideAudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? StatusName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SectionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Box Box
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public AudioFile GuideAudio
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<Section> Sections
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 150)
                errors.AddError("Title cannot be longer than 150 characters.", nameof(Title));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!ShowNotebook.HasValue)
                errors.AddError("Show Notebook is required.", nameof(ShowNotebook));

            if (!ShowGuide.HasValue)
                errors.AddError("Show Guide is required.", nameof(ShowGuide));

            errors.AddRange(Box.Validate());
        });
    }
}
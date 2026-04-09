namespace Crudspa.Education.Student.Shared.Contracts.Data;

public class Book : Observable, IUnique, IValidates
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

    public String? Isbn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Author
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Lexile
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SeasonId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? StatusId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Summary
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CoverImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CoverImageUri
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? ShowOnline
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public String? YouTubeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? SongAudioFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CategoryId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? IntroVideoId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public VideoFile IntroVideo
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Guid? ReadAloudVideoId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GuideImageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? RequiresAchievementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public VideoFile ReadAloudVideo
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public BookCategory Category
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile CoverImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? BookCategoryName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? BookCategoryOrdinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile SongAudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean? Selected
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Version
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? LastAccessed
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable> SchoolYears
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> Grades
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Guid? PrefaceBinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public BookProgress Progress
    {
        get;
        set => SetProperty(ref field, value);
    } = new() { ContentCompletedCount = 0, MapCompletedCount = 0, PrefaceCompletedCount = 0 };

    public ImageFile GuideImage
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<Game> Games
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Module> Modules
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Boolean? HasPreface
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? HasContent
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? HasMap
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (String.IsNullOrWhiteSpace(Key))
                errors.AddError("Key is required.", nameof(Key));
            else if (Key.Length > 100)
                errors.AddError("Key cannot be longer than 100 characters.", nameof(Key));

            if (String.IsNullOrWhiteSpace(Title))
                errors.AddError("Title is required.", nameof(Title));
            else if (Title.Length > 150)
                errors.AddError("Title cannot be longer than 150 characters.", nameof(Title));

            if (String.IsNullOrWhiteSpace(Author))
                errors.AddError("Author is required.", nameof(Author));
            else if (Author.Length > 150)
                errors.AddError("Author cannot be longer than 150 characters.", nameof(Author));

            if (String.IsNullOrWhiteSpace(Isbn))
                errors.AddError("ISBN is required.", nameof(Isbn));
            else if (Isbn.Length > 20)
                errors.AddError("ISBN cannot be longer than 20 characters.", nameof(Isbn));

            if (String.IsNullOrWhiteSpace(Lexile))
                errors.AddError("Lexile is required.", nameof(Lexile));
            else if (Lexile.Length > 10)
                errors.AddError("Lexile cannot be longer than 10 characters.", nameof(Lexile));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (!SeasonId.HasValue)
                errors.AddError("Season is required.", nameof(SeasonId));

            if (YouTubeId.HasSomething() && YouTubeId!.Length > 20)
                errors.AddError("YouTube ID cannot be longer than 20 characters.", nameof(YouTubeId));

            if (CoverImage.Name is null)
                errors.AddError("Cover Image is required.", nameof(CoverImage));
            else
                errors.AddRange(CoverImage.Validate());

            if (GuideImage.BlobId is null)
                errors.AddError("Guide Image is required.", nameof(GuideImage));
            else
                errors.AddRange(GuideImage.Validate());
        });
    }
}
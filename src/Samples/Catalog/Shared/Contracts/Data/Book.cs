namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class Book : Observable, IValidates, INamed, ICountable
{
    public String? Name => Title;

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

    public String? Isbn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Author
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? GenreId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? GenreName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Pages
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Single? Price
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Summary
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile CoverImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public PdfFile SamplePdfFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public AudioFile PreviewAudioFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<Selectable> Tags
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<BookEdition> BookEditions
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Isbn.HasNothing())
                errors.AddError("Isbn is required.", nameof(Isbn));
            else if (Isbn!.Length > 20)
                errors.AddError("Isbn cannot be longer than 20 characters.", nameof(Isbn));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 160)
                errors.AddError("Title cannot be longer than 160 characters.", nameof(Title));

            if (Author.HasNothing())
                errors.AddError("Author is required.", nameof(Author));
            else if (Author!.Length > 120)
                errors.AddError("Author cannot be longer than 120 characters.", nameof(Author));

            if (!GenreId.HasValue)
                errors.AddError("Genre is required.", nameof(GenreId));

            if (!Pages.HasValue)
                errors.AddError("Pages is required.", nameof(Pages));

            if (!Price.HasValue)
                errors.AddError("Price is required.", nameof(Price));

            BookEditions.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}
namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class Book : Observable, IValidates, INamed, ICountable, IRelates
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

    public String? Key
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Author
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Isbn
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

    public String? SeasonName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CategoryId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CategoryName
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

    public ImageFile GuideImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? ChapterCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? GameCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ModuleCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TrifoldCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<RelatedEntity> Relations
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PrefaceBinderId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PrefaceCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Key.HasNothing())
                errors.AddError("Key is required.", nameof(Key));
            else if (Key!.Length > 100)
                errors.AddError("Key cannot be longer than 100 characters.", nameof(Key));

            if (Isbn.HasNothing())
                errors.AddError("ISBN is required.", nameof(Isbn));
            else if (Isbn!.Length > 20)
                errors.AddError("ISBN cannot be longer than 20 characters.", nameof(Isbn));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 150)
                errors.AddError("Title cannot be longer than 150 characters.", nameof(Title));

            if (Author.HasNothing())
                errors.AddError("Author is required.", nameof(Author));
            else if (Author!.Length > 150)
                errors.AddError("Author cannot be longer than 150 characters.", nameof(Author));

            if (Lexile.HasNothing())
                errors.AddError("Lexile is required.", nameof(Lexile));
            else if (Lexile!.Length > 10)
                errors.AddError("Lexile cannot be longer than 10 characters.", nameof(Lexile));

            if (!SeasonId.HasValue)
                errors.AddError("Season is required.", nameof(SeasonId));

            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (CoverImageFile.Name.HasNothing())
                errors.AddError("Cover Image is required.", nameof(CoverImageFile));
        });
    }
}
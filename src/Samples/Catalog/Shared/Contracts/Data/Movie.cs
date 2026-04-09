namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class Movie : Observable, IValidates, INamed, ICountable
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

    public Guid? RatingId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? RatingName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Released
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? RuntimeMin
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Single? Score
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Summary
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile PosterImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public VideoFile TrailerVideoFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? MovieCreditCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
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
            else if (Title!.Length > 160)
                errors.AddError("Title cannot be longer than 160 characters.", nameof(Title));

            if (!GenreId.HasValue)
                errors.AddError("Genre is required.", nameof(GenreId));

            if (!RatingId.HasValue)
                errors.AddError("Rating is required.", nameof(RatingId));

            if (!RuntimeMin.HasValue)
                errors.AddError("Runtime Min is required.", nameof(RuntimeMin));
        });
    }
}
namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Post : Observable, IValidates, ICountable, INamed
{
    public enum CommentRules { Disabled, Show, Allow }

    private String? _title;

    public String? Name
    {
        get => _title;
        set => SetProperty(ref _title, value);
    }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BlogId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BlogTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get => _title;
        set => SetProperty(ref _title, value);
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

    public String? Author
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? Published
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? Revised
    {
        get;
        set => SetProperty(ref field, value);
    }

    public CommentRules CommentRule
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Page Page
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? CommentCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PostReactionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? PostTagCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? SectionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PageId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!StatusId.HasValue)
                errors.AddError("Status is required.", nameof(StatusId));

            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 150)
                errors.AddError("Title cannot be longer than 150 characters.", nameof(Title));

            if (Author.HasNothing())
                errors.AddError("Author is required.", nameof(Author));
            else if (Author!.Length > 150)
                errors.AddError("Author cannot be longer than 150 characters.", nameof(Author));

            if (!Published.HasValue)
                errors.AddError("Published is required.", nameof(Published));
        });
    }
}
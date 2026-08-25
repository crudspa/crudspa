namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Thread : Observable, IValidates, ICountable, INamed
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ForumId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ForumTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Pinned
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Comment Comment
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ObservableCollection<ForumBundle> ForumBundles
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Selectable> Tags
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32? CommentCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? LastActivity
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean CanEdit
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean CanDelete
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean CanModerate
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get => Title;
        set => Title = value;
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 150)
                errors.AddError("Title cannot be longer than 150 characters.", nameof(Title));

            if (!Pinned.HasValue)
                errors.AddError("Pinned is required.", nameof(Pinned));

            if (Comment.Body?.Length > ForumPolicy.MaxBodyCharacters)
                errors.AddError($"Body cannot be longer than {ForumPolicy.MaxBodyCharacters:N0} characters.", nameof(Comment.Body));

            if (Comment.CommentMedias.Count > ForumMediaPolicy.MaxAttachmentsPerComment)
                errors.AddError($"A discussion can have at most {ForumMediaPolicy.MaxAttachmentsPerComment} media items.", nameof(Comment.CommentMedias));

            errors.AddRange(Comment.Validate());
            errors.AddRange(ForumBundle.ValidateSelection(ForumBundles, Tags, false, nameof(Tags)));
        });
    }
}
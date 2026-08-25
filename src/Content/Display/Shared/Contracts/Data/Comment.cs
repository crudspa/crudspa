namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Comment : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PostId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ParentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ParentBody
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Body
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ById
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ByFirstName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ByName
    {
        get => ByFirstName;
        set => ByFirstName = value;
    }

    public String? ByOrganizationName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Posted
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Edited
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Removed
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

    public Boolean CanReply
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ThreadId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<CommentMedia> CommentMedias
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<Comment> Children
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<CommentReaction> Reactions
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

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

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!Removed && Body.HasNothing())
                errors.AddError("Body is required.", nameof(Body));
            else if (!Removed && ThreadId.HasValue && Body!.Length > ForumPolicy.MaxBodyCharacters)
                errors.AddError($"Body cannot be longer than {ForumPolicy.MaxBodyCharacters:N0} characters.", nameof(Body));

            if (ThreadId.HasValue && CommentMedias.Count > ForumMediaPolicy.MaxAttachmentsPerComment)
                errors.AddError($"A comment can have at most {ForumMediaPolicy.MaxAttachmentsPerComment} media items.", nameof(CommentMedias));

            CommentMedias.Apply(x => errors.AddRange(x.Validate()));
            errors.AddRange(ForumBundle.ValidateSelection(ForumBundles, Tags, true, nameof(Tags)));
        });
    }
}
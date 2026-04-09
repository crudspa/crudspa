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

    public Int32? CommentCount
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

            errors.AddRange(Comment.Validate());
        });
    }
}
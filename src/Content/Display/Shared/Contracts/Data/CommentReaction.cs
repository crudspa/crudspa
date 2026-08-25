namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class CommentReaction : Observable, IValidates
{
    public Guid? CommentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Emoji
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32 Count
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean Selected
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!CommentId.HasValue)
                errors.AddError("Comment is required.", nameof(CommentId));

            if (Emoji is not null && (Emoji.HasNothing() || Emoji.Length > 2))
                errors.AddError("Reaction is invalid.", nameof(Emoji));
        });
    }
}
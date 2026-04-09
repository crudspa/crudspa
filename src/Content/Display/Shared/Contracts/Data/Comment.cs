namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Comment : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ThreadId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ThreadTitle
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

    public String? ByName
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

    public ObservableCollection<CommentMedia> CommentMedias
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Body.HasNothing())
                errors.AddError("Body is required.", nameof(Body));

            CommentMedias.Apply(x => errors.AddRange(x.Validate()));
        });
    }
}
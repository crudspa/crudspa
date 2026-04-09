namespace Crudspa.Content.Display.Shared.Contracts.Config.ElementType;

public class TextElement : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Text.HasNothing())
                errors.AddError("Text is required.", nameof(Text));
        });
    }
}
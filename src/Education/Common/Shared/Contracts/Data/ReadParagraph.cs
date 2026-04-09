namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ReadParagraph : Observable, IValidates, IOrderable, INamed
{
    public String Name => $"Paragraph {Ordinal + 1:D}";

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ReadPartId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ReadPartTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
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
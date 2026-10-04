namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class PublicationResourceSummary : Observable, IValidates, IOrderable
{

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PublicationId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PublicationTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Label
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Url
    {
        get;
        set => SetProperty(ref field, value);
    }

    public PdfFile PdfFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }


    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));
        });
    }
}
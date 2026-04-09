namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class EmailAttachment : Observable, IValidates, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? EmailId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? EmailFromName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public PdfFile PdfFile
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
            if (PdfFile.Name.HasNothing() || (!PdfFile.BlobId.HasValue && !PdfFile.Id.HasValue))
                errors.AddError("PDF file is required.", nameof(PdfFile));
        });
    }
}
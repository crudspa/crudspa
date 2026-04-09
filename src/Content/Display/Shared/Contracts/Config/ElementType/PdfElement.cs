namespace Crudspa.Content.Display.Shared.Contracts.Config.ElementType;

public class PdfElement : Observable, IValidates
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

    public PdfFile FileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (FileFile.Name.HasNothing() || (!FileFile.BlobId.HasValue && !FileFile.Id.HasValue))
                errors.AddError("PDF File is required.", nameof(FileFile));
        });
    }
}
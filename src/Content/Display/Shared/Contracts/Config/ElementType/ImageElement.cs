namespace Crudspa.Content.Display.Shared.Contracts.Config.ElementType;

public class ImageElement : Observable, IValidates
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

    public ImageFile FileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? HyperlinkUrl
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (FileFile.Name.HasNothing() || (!FileFile.BlobId.HasValue && !FileFile.Id.HasValue))
                errors.AddError("Image File is required.", nameof(FileFile));

            if (HyperlinkUrl.HasSomething() && !HyperlinkUrl.IsValidHyperlink())
                errors.AddError("Hyperlink URL must be a valid internal link (starting with '/') or external link (starting with https://) and be a properly formatted URL.", nameof(HyperlinkUrl));
        });
    }
}

namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class PublicationResource : Observable, IValidates, IOrderable
{

    public String? Name => Label ?? TypeName ?? "Resource";

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
            if ((!String.IsNullOrWhiteSpace(PdfFile.Name) && !PdfFile.Id.HasValue && !PdfFile.BlobId.HasValue)
                || (!String.IsNullOrWhiteSpace(ImageFile.Name) && !ImageFile.Id.HasValue && !ImageFile.BlobId.HasValue))
                errors.AddError("Wait for the resource upload to finish before saving.");
            var destinations = (String.IsNullOrWhiteSpace(Url) ? 0 : 1)
                + (PdfFile.Id.HasValue || PdfFile.BlobId.HasValue || !String.IsNullOrWhiteSpace(PdfFile.Name) ? 1 : 0)
                + (ImageFile.Id.HasValue || ImageFile.BlobId.HasValue || !String.IsNullOrWhiteSpace(ImageFile.Name) ? 1 : 0);
            if (destinations != 1)
                errors.AddError("Choose exactly one destination: link, PDF, or image.");
            if (!String.IsNullOrWhiteSpace(Url) && (!Uri.TryCreate(Url.Trim(), UriKind.Absolute, out var uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps)))
                errors.AddError("Enter a complete HTTP or HTTPS link.", nameof(Url));
            if (Url?.Length > 2000 || Label?.Length > 150)
                errors.AddError("The resource link or label is too long.");
            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));
        });
    }
}
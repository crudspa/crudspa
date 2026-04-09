namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class Shirt : Observable, IValidates, INamed, ICountable
{
    public enum Fits { Slim, Regular, Oversized }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BrandId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BrandName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Fits Fit
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Material
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Single? Price
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile HeroImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public PdfFile GuidePdfFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? ShirtOptionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? TotalCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 120)
                errors.AddError("Name cannot be longer than 120 characters.", nameof(Name));

            if (!BrandId.HasValue)
                errors.AddError("Brand is required.", nameof(BrandId));

            if (!Price.HasValue)
                errors.AddError("Price is required.", nameof(Price));
        });
    }
}
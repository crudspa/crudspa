namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class ShirtOption : Observable, IValidates, INamed, IOrderable
{
    public String? Name => ColorName;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ShirtId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? SkuBase
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Single? Price
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ColorId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ColorName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? AllSizes
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<Selectable> Sizes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (SkuBase.HasNothing())
                errors.AddError("Sku Base is required.", nameof(SkuBase));
            else if (SkuBase!.Length > 40)
                errors.AddError("Sku Base cannot be longer than 40 characters.", nameof(SkuBase));

            if (!Price.HasValue)
                errors.AddError("Price is required.", nameof(Price));
        });
    }
}
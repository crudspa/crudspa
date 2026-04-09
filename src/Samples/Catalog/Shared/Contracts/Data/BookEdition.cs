namespace Crudspa.Samples.Catalog.Shared.Contracts.Data;

public class BookEdition : Observable, IValidates, IOrderable
{

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? FormatId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? FormatName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Sku
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Single? Price
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateOnly? ReleasedOn
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? InPrint
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }


    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!FormatId.HasValue)
                errors.AddError("Format is required.", nameof(FormatId));

            if (Sku.HasNothing())
                errors.AddError("Sku is required.", nameof(Sku));
            else if (Sku!.Length > 40)
                errors.AddError("Sku cannot be longer than 40 characters.", nameof(Sku));

            if (!Price.HasValue)
                errors.AddError("Price is required.", nameof(Price));

            if (!InPrint.HasValue)
                errors.AddError("In Print is required.", nameof(InPrint));
        });
    }
}
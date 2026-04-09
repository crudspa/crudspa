namespace Crudspa.Education.Publisher.Shared.Contracts.Data;

public class UnitBook : Observable, IValidates, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? UnitId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BookId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Treatment
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Control
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BookKey
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BookTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile? BookCoverImage
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
            if (!BookId.HasValue)
                errors.AddError("Book is required.", nameof(BookId));

            if (!Treatment.HasValue)
                errors.AddError("Treatment is required.", nameof(Treatment));

            if (!Control.HasValue)
                errors.AddError("Control is required.", nameof(Control));
        });
    }
}
using Crudspa.Content.Display.Shared.Contracts.Ids;

namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Item : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BasisId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? BasisAmount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Grow
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Shrink
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AlignSelfId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MaxWidth
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MinWidth
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Width
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!BasisId.HasValue)
                errors.AddError("Basis is required.", nameof(BasisId));
            else
            {
                if (BasisId.Equals(BasisIds.Fixed))
                {
                    if (BasisAmount.HasNothing())
                        errors.AddError("Basis Amount is required.", nameof(BasisAmount));
                    else
                        BasisAmount.ValidateResponsiveLengthProperty(nameof(BasisAmount), errors);
                }
                else if (BasisId.Equals(BasisIds.Percentage))
                {
                    if (BasisAmount.HasNothing())
                        errors.AddError("Basis Amount is required.", nameof(BasisAmount));
                    else if (!BasisAmount.IsValidCssPercentage())
                        errors.AddError("Basis Amount must be a number followed by '%'.", nameof(BasisAmount));
                }
            }

            if (!Grow.HasSomething())
                errors.AddError("Grow is required.", nameof(Grow));
            else if (!Grow.IsValidCssPositiveNumber())
                errors.AddError("Grow must be 0 or a positive number.", nameof(Grow));

            if (!Shrink.HasSomething())
                errors.AddError("Shrink is required.", nameof(Shrink));
            else if (!Shrink.IsValidCssPositiveNumber())
                errors.AddError("Shrink must be 0 or a positive number.", nameof(Shrink));

            if (!AlignSelfId.HasValue)
                errors.AddError("Align Self is required.", nameof(AlignSelfId));

            MaxWidth.ValidateResponsiveWidthProperty(nameof(MaxWidth), errors);
            MinWidth.ValidateResponsiveWidthProperty(nameof(MinWidth), errors);
            Width.ValidateResponsiveWidthProperty(nameof(Width), errors);
        });
    }
}
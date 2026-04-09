namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Container : Observable, IValidates
{
    public String? Name => null;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? DirectionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? WrapId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? JustifyContentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AlignItemsId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AlignContentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Gap
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!DirectionId.HasValue)
                errors.AddError("Direction is required.", nameof(DirectionId));

            if (!WrapId.HasValue)
                errors.AddError("Wrap is required.", nameof(WrapId));

            if (!JustifyContentId.HasValue)
                errors.AddError("Justify Content is required.", nameof(JustifyContentId));

            if (!AlignItemsId.HasValue)
                errors.AddError("Align Items is required.", nameof(AlignItemsId));

            if (!AlignContentId.HasValue)
                errors.AddError("Align Content is required.", nameof(AlignContentId));

            Gap.ValidateLengthOrPercentageProperty(nameof(Gap), errors);
        });
    }
}
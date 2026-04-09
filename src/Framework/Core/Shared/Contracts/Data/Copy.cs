namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class Copy : Observable, IValidates
{
    public Guid? ExistingId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ExistingParentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? NewId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? NewParentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? NewName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public virtual List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (ExistingId.HasNothing())
                errors.AddError("Existing Id is required.", nameof(ExistingId));

            if (NewName.HasNothing())
                errors.AddError("Name is required.", nameof(NewName));
        });
    }
}
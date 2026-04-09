namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ExportFile : Observable, INamed
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Format
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Description
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Published
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!BlobId.HasValue)
                errors.AddError("File is required.", nameof(BlobId));

            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name.Length > 150)
                errors.AddError("Name cannot be longer than 150 characters.", nameof(Name));
        });
    }
}
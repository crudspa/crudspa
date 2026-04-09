namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Font : Observable, IValidates, INamed
{
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

    public Guid? ContentPortalId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public FontFile FileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 75)
                errors.AddError("Name cannot be longer than 75 characters.", nameof(Name));

            if (FileFile.Name.HasNothing() || (!FileFile.BlobId.HasValue && !FileFile.Id.HasValue))
                errors.AddError("File is required.", nameof(FileFile));
        });
    }
}
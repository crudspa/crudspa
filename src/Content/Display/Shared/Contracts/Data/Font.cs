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

    public List<FontFace> Faces
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 75)
                errors.AddError("Name cannot be longer than 75 characters.", nameof(Name));

            if (Faces.IsEmpty())
                errors.AddError("At least one font face is required.", nameof(Faces));
        });
    }
}
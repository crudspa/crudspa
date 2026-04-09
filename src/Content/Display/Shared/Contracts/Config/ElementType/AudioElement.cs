namespace Crudspa.Content.Display.Shared.Contracts.Config.ElementType;

public class AudioElement : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile FileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (FileFile.Name.HasNothing() || (!FileFile.BlobId.HasValue && !FileFile.Id.HasValue))
                errors.AddError("Audio File is required.", nameof(FileFile));
        });
    }
}
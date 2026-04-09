namespace Crudspa.Content.Display.Shared.Contracts.Config.ElementType;

public class VideoElement : Observable, IValidates
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

    public VideoFile FileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean? AutoPlay
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public ImageFile Poster
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (FileFile.Name.HasNothing() || (!FileFile.BlobId.HasValue && !FileFile.Id.HasValue))
                errors.AddError("Video File is required.", nameof(FileFile));
        });
    }
}
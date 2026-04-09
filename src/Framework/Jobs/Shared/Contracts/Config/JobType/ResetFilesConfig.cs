namespace Crudspa.Framework.Jobs.Shared.Contracts.Config.JobType;

public class ResetFilesConfig : Observable
{
    public Boolean? OptimizedAudioFiles
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? OptimizedImageFiles
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? ImageCaptions
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public Boolean? OptimizedVideoFiles
    {
        get;
        set => SetProperty(ref field, value);
    } = false;

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (OptimizedAudioFiles != true && OptimizedImageFiles != true && ImageCaptions != true && OptimizedVideoFiles != true)
                errors.AddError("Reset what?");
        });
    }
}
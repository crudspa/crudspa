using System.Text.Json.Serialization;

namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class AudioFile : Observable, INamed, IValidates
{
    public enum OptimizationStatus { None, Succeeded, Failed }

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

    public OptimizationStatus OptimizedStatus
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? OptimizedBlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? OptimizedFormat
    {
        get;
        set => SetProperty(ref field, value);
    }

    [JsonIgnore] public String? UploadPreview
    {
        get;
        set => SetProperty(ref field, value);
    }

    [JsonIgnore] public Int32 UploadProgress
    {
        get;
        set => SetProperty(ref field, value);
    }

    [JsonIgnore] public String? UploadStatus
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!BlobId.HasValue)
                errors.AddError("Blob is required.", nameof(BlobId));

            if (Name.HasNothing())
                errors.AddError("Name is required.", nameof(Name));
            else if (Name!.Length > 150)
                errors.AddError("Name cannot be longer than 150 characters.", nameof(Name));

            if (OptimizedFormat.HasSomething() && OptimizedFormat!.Length > 10)
                errors.AddError("Optimized Format cannot be longer than 10 characters.", nameof(OptimizedFormat));
        });
    }
}
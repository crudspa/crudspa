using System.Text.Json.Serialization;

namespace Crudspa.Framework.Core.Shared.Contracts.Data;

public class ImageFile : Observable, INamed, IValidates
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

    public Int32? Width
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public Int32? Height
    {
        get;
        set => SetProperty(ref field, value);
    } = 0;

    public String? Caption
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

    public Guid? Resized96BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Resized192BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Resized360BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Resized540BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Resized720BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Resized1080BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Resized1440BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Resized1920BlobId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? Resized3840BlobId
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
                errors.AddError("Image is required.", nameof(BlobId));
        });
    }
}
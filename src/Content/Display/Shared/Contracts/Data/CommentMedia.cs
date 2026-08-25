namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class CommentMedia : Observable, IValidates, IOrderable
{
    public enum Types { Audio, Image, Pdf, Video }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CommentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CommentBody
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Types Type
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile AudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public PdfFile PdfFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public VideoFile VideoFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Name => Type switch
    {
        Types.Audio => AudioFile.Name,
        Types.Image => ImageFile.Name,
        Types.Pdf => PdfFile.Name,
        Types.Video => VideoFile.Name,
        _ => null,
    };

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            var selectedCount = 0;

            if (HasFileReference(AudioFile)) selectedCount++;
            if (HasFileReference(ImageFile)) selectedCount++;
            if (HasFileReference(PdfFile)) selectedCount++;
            if (HasFileReference(VideoFile)) selectedCount++;

            var hasAnyRawIdentifier = AudioFile.Id.HasValue
                                      || AudioFile.BlobId.HasValue
                                      || AudioFile.OptimizedBlobId.HasValue
                                      || ImageFile.Id.HasValue
                                      || ImageFile.BlobId.HasValue
                                      || ImageFile.OptimizedBlobId.HasValue
                                      || ImageFile.Resized96BlobId.HasValue
                                      || ImageFile.Resized192BlobId.HasValue
                                      || ImageFile.Resized360BlobId.HasValue
                                      || ImageFile.Resized540BlobId.HasValue
                                      || ImageFile.Resized720BlobId.HasValue
                                      || ImageFile.Resized1080BlobId.HasValue
                                      || ImageFile.Resized1440BlobId.HasValue
                                      || ImageFile.Resized1920BlobId.HasValue
                                      || ImageFile.Resized3840BlobId.HasValue
                                      || PdfFile.Id.HasValue
                                      || PdfFile.BlobId.HasValue
                                      || VideoFile.Id.HasValue
                                      || VideoFile.BlobId.HasValue
                                      || VideoFile.OptimizedBlobId.HasValue
                                      || VideoFile.PosterBlobId.HasValue;

            var isRedactedExistingAttachment = Id.HasValue
                                                && !hasAnyRawIdentifier
                                                && (Type switch
                                                {
                                                    Types.Audio => AudioFile.Name.HasSomething(),
                                                    Types.Image => ImageFile.Name.HasSomething(),
                                                    Types.Pdf => PdfFile.Name.HasSomething(),
                                                    Types.Video => VideoFile.Name.HasSomething(),
                                                    _ => false,
                                                });

            if (isRedactedExistingAttachment)
                return;

            if (selectedCount != 1)
            {
                errors.AddError("Exactly one media file is required.", nameof(Type));
                return;
            }

            switch (Type)
            {
                case Types.Audio when AudioFile.Id.HasValue || AudioFile.BlobId.HasValue:
                    errors.AddRange(AudioFile.Validate());
                    break;
                case Types.Image when ImageFile.Id.HasValue || ImageFile.BlobId.HasValue:
                    errors.AddRange(ImageFile.Validate());
                    break;
                case Types.Pdf when PdfFile.Id.HasValue || PdfFile.BlobId.HasValue:
                    errors.AddRange(PdfFile.Validate());
                    break;
                case Types.Video when VideoFile.Id.HasValue || VideoFile.BlobId.HasValue:
                    errors.AddRange(VideoFile.Validate());
                    break;
                default:
                    errors.AddError("Media type must match the selected file.", nameof(Type));
                    break;
            }
        });
    }

    private static Boolean HasFileReference(AudioFile file) =>
        file.Id.HasValue || file.BlobId.HasValue || file.OptimizedBlobId.HasValue;

    private static Boolean HasFileReference(ImageFile file) =>
        file.Id.HasValue
        || file.BlobId.HasValue
        || file.OptimizedBlobId.HasValue
        || file.Resized96BlobId.HasValue
        || file.Resized192BlobId.HasValue
        || file.Resized360BlobId.HasValue
        || file.Resized540BlobId.HasValue
        || file.Resized720BlobId.HasValue
        || file.Resized1080BlobId.HasValue
        || file.Resized1440BlobId.HasValue
        || file.Resized1920BlobId.HasValue
        || file.Resized3840BlobId.HasValue;

    private static Boolean HasFileReference(PdfFile file) =>
        file.Id.HasValue || file.BlobId.HasValue;

    private static Boolean HasFileReference(VideoFile file) =>
        file.Id.HasValue || file.BlobId.HasValue || file.OptimizedBlobId.HasValue || file.PosterBlobId.HasValue;
}
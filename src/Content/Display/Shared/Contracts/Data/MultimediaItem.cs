namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class MultimediaItem : Observable, IValidates, IOrderable
{
    public enum MediaTypes
    {
        Audio,
        Button,
        Image,
        Text,
        Video,
    }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? MultimediaElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Box Box
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Item Item
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public MediaTypes MediaTypeIndex
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile AudioFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Button Button
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

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

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            switch (MediaTypeIndex)
            {
                case MediaTypes.Audio:
                    if (AudioFile.Name.HasNothing())
                        errors.AddError("Audio File is required.", nameof(AudioFile));
                    else
                        errors.AddRange(AudioFile.Validate());
                    break;
                case MediaTypes.Button:
                    errors.AddRange(Button.Validate());
                    break;
                case MediaTypes.Image:
                    if (ImageFile.Name.HasNothing())
                        errors.AddError("Image File is required.", nameof(ImageFile));
                    else
                        errors.AddRange(ImageFile.Validate());
                    break;
                case MediaTypes.Text:
                    if (Text.HasNothing())
                        errors.AddError("Text is required.", nameof(Text));
                    break;
                case MediaTypes.Video:
                    if (VideoFile.Name.HasNothing())
                        errors.AddError("Video File is required.", nameof(VideoFile));
                    else
                        errors.AddRange(VideoFile.Validate());

                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        });
    }
}
namespace Crudspa.Content.Display.Shared.Contracts.Data;

public class Button : Observable, IValidates
{
    public enum Shapes { Rectangle, Square }
    public enum Graphics { None, Icon, Image }
    public enum TextTypes { None, Custom }
    public enum Orientations { Left, Right }

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? Internal
    {
        get;
        set => SetProperty(ref field, value);
    } = true;

    public String? Path
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Shapes ShapeIndex
    {
        get;
        set => SetProperty(ref field, value);
    } = Shapes.Rectangle;

    public Graphics GraphicIndex
    {
        get;
        set => SetProperty(ref field, value);
    } = Graphics.Icon;

    public TextTypes TextTypeIndex
    {
        get;
        set => SetProperty(ref field, value);
    } = TextTypes.Custom;

    public Orientations OrientationIndex
    {
        get;
        set => SetProperty(ref field, value);
    } = Orientations.Left;

    public Guid? IconId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Box Box
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? IconCssClass
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!Internal.HasValue)
                errors.AddError("Internal is required.", nameof(Internal));

            if (Path.HasNothing())
                errors.AddError("Path is required.", nameof(Path));
            else if (Path!.Length > 250)
                errors.AddError("Path cannot be longer than 250 characters.", nameof(Path));

            if (TextTypeIndex == TextTypes.Custom && Text.HasNothing())
                errors.AddError("Text is required when text is enabled.", nameof(Text));
            else if (TextTypeIndex == TextTypes.Custom && Text!.Length > 250)
                errors.AddError("Text cannot be longer than 250 characters.", nameof(Text));

            if (GraphicIndex == Graphics.Icon && !IconId.HasValue)
                errors.AddError("Icon is required when graphic is set to Icon.", nameof(IconId));

            if (GraphicIndex == Graphics.Image)
                errors.AddRange(ImageFile.Validate());

            if (GraphicIndex == Graphics.None && TextTypeIndex == TextTypes.None)
                errors.AddError("Button must include text or a graphic.", nameof(GraphicIndex));
        });
    }
}
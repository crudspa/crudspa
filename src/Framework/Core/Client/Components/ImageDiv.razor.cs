namespace Crudspa.Framework.Core.Client.Components;

public partial class ImageDiv
{
    [Parameter, EditorRequired] public ImageFile? ImageFile { get; set; }
    [Parameter] public Int32? Width { get; set; }

    public String ImageUrl => ImageFile.FetchUrl(Width);
}
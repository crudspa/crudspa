namespace Crudspa.Content.Design.Client.Plugins.ElementType;

public partial class ImageElementDesign : IElementDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    public ImageElement ImageElement { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        ImageElement = Element.RequireConfig<ImageElement>();
        return Task.CompletedTask;
    }

    public void PrepareForSave() { }
}
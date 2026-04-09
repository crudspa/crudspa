namespace Crudspa.Content.Design.Client.Plugins.ElementType;

public partial class VideoElementDesign : IElementDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    public VideoElement VideoElement { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        VideoElement = Element.RequireConfig<VideoElement>();
        return Task.CompletedTask;
    }

    public void PrepareForSave() { }
}
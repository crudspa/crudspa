namespace Crudspa.Content.Design.Client.Plugins.ElementType;

public partial class AudioElementDesign : IElementDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    public AudioElement AudioElement { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        AudioElement = Element.RequireConfig<AudioElement>();
        return Task.CompletedTask;
    }

    public void PrepareForSave() { }
}
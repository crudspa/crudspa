namespace Crudspa.Content.Design.Client.Plugins.ElementType;

public partial class PdfElementDesign : IElementDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    public PdfElement Pdf { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Pdf = Element.RequireConfig<PdfElement>();
        return Task.CompletedTask;
    }

    public void PrepareForSave() { }
}
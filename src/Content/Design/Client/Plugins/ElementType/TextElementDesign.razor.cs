using HtmlEditorMarkup = Crudspa.Framework.Core.Client.Components.HtmlEditorMarkup;

namespace Crudspa.Content.Design.Client.Plugins.ElementType;

public partial class TextElementDesign : IElementDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    public TextElement TextElement { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        TextElement = Element.RequireConfig<TextElement>();
        return Task.CompletedTask;
    }

    public void PrepareForSave()
    {
        TextElement.Text = HtmlEditorMarkup.NormalizeForStorage(TextElement.Text);
    }
}
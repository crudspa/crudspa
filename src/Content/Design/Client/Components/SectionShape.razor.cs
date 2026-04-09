namespace Crudspa.Content.Design.Client.Components;

public partial class SectionShape
{
    [Parameter, EditorRequired] public SectionEditModel Model { get; set; } = null!;

    private Section? PreviewSection => Model.SectionShapeModel.PreviewSection;
}
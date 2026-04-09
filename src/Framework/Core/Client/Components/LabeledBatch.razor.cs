namespace Crudspa.Framework.Core.Client.Components;

public partial class LabeledBatch<T>
    where T : class, IObservable
{
    [Parameter, EditorRequired] public String Label { get; set; } = null!;
    [Parameter, EditorRequired] public IEnumerable<T> Items { get; set; } = null!;
    [Parameter, EditorRequired] public RenderFragment<T> ReadView { get; set; } = null!;
    [Parameter] public String Width { get; set; } = "8em";
}
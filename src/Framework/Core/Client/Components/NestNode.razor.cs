namespace Crudspa.Framework.Core.Client.Components;

public partial class NestNode
{
    [Parameter, EditorRequired] public Expandable Node { get; set; } = null!;
    [Parameter, EditorRequired] public EventCallback<Guid?> SelectionChanged { get; set; }
}
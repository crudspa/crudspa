namespace Crudspa.Framework.Core.Client.Components;

public partial class NoRecords
{
    [Parameter, EditorRequired] public String EntityName { get; set; } = null!;
}
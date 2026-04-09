namespace Crudspa.Framework.Core.Client.Components;

public partial class BatchMenuItems
{
    [Parameter] public EventCallback MoveUpClicked { get; set; }
    [Parameter] public EventCallback MoveDownClicked { get; set; }
    [Parameter] public EventCallback MoveLeftClicked { get; set; }
    [Parameter] public EventCallback MoveRightClicked { get; set; }
    [Parameter] public EventCallback RemoveClicked { get; set; }
}
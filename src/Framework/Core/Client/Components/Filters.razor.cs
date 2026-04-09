namespace Crudspa.Framework.Core.Client.Components;

public partial class Filters
{
    [Parameter] public RenderFragment Fields { get; set; } = null!;
    [Parameter] public RenderFragment Buttons { get; set; } = null!;
}
using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class MultiSelect
{
    [Parameter, EditorRequired] public IEnumerable<Named> LookupValues { get; set; } = null!;
    [Parameter] public G.List<Guid?> SelectedValues { get; set; } = null!;
    [Parameter] public EventCallback<G.List<Guid?>> SelectedValuesChanged { get; set; }
}
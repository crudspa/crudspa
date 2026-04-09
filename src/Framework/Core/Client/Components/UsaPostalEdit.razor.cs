using G = System.Collections.Generic;

namespace Crudspa.Framework.Core.Client.Components;

public partial class UsaPostalEdit
{
    [Parameter, EditorRequired] public UsaPostal? UsaPostal { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public RenderFragment? ChildContent { get; set; }
    [Parameter] public Boolean ShowRecipient { get; set; }
    [Parameter] public Boolean ShowBusiness { get; set; }

    [Inject] public IAddressService AddressService { get; set; } = null!;

    public G.List<Named>? UsaStateNames { get; set; }

    protected override async Task OnInitializedAsync()
    {
        var response = await AddressService.FetchUsaStateNames(new());
        if (response.Ok) UsaStateNames = response.Value.ToList();
    }
}
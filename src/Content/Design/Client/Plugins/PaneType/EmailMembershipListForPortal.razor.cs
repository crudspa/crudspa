namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class EmailMembershipListForPortal : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IMembershipService MembershipService { get; set; } = null!;

    public MembershipListForPortalModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        Model = new(EventBus, ScrollService, MembershipService, Id);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Refresh();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}
using Crudspa.Content.Display.Client.Contracts.Events;

namespace Crudspa.Content.Display.Client.Components;

public partial class MyAchievements : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public MyAchievementsModel Model { get; set; } = null!;

    [Inject] public ISessionState SessionState { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class MyAchievementsModel(
    IScrollService scrollService,
    IEventBus eventBus,
    IContactAchievementService contactAchievementService)
    : ModalModel(scrollService)
{
    public ObservableCollection<ContactAchievement>? ContactAchievements
    {
        get;
        set => SetProperty(ref field, value);
    }

    public override async Task Show()
    {
        await base.Show();

        var response = await WithWaiting("Working...", () => contactAchievementService.FetchAchievements(new()));

        if (response.Ok)
            ContactAchievements = response.Value.ToObservable();
    }

    public override Task Hide()
    {
        ContactAchievements = null;
        return base.Hide();
    }

    public void ViewAchievement(ContactAchievement contactAchievement)
    {
        contactAchievement.IsNew = false;
        eventBus.Publish(new ShowAchievement { ContactAchievement = contactAchievement });
    }
}
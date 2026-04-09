namespace Crudspa.Content.Display.Client.Components;

public partial class Achievement : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter, EditorRequired] public AchievementModel Model { get; set; } = null!;

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

public class AchievementModel(
    IScrollService scrollService,
    IContactAchievementService contactAchievementService)
    : ModalModel(scrollService)
{
    private Guid? _id;
    private Boolean _isNew;

    public ContactAchievement? ContactAchievement
    {
        get;
        set => SetProperty(ref field, value);
    }

    public async Task Show(ContactAchievement contactAchievement)
    {
        _id = contactAchievement.Id;
        _isNew = contactAchievement.IsNew;

        await Show();
    }

    public override async Task Show()
    {
        if (_id is null)
            return;

        Title = "Achievement";

        await base.Show();

        var response = await WithWaiting("Working...", () => contactAchievementService.FetchAchievement(new(new() { Id = _id })));

        if (response.Ok)
        {
            var contactAchievement = response.Value;

            contactAchievement.IsNew = _isNew;

            ContactAchievement = contactAchievement;

            if (ContactAchievement.IsNew)
                Title = "Achievement Unlocked!";
        }
    }

    public override Task Hide()
    {
        ContactAchievement = null;
        return base.Hide();
    }

    public Boolean HasUnlocks
    {
        get
        {
            if (ContactAchievement is null)
                return false;

            return ContactAchievement.Unlocks.Tracks.Count > 0
                || ContactAchievement.Unlocks.Courses.Count > 0;
        }
    }
}
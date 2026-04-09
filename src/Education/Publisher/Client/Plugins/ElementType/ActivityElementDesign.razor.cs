using Crudspa.Education.Common.Shared.Contracts.Config.ElementType;
using Crudspa.Education.Publisher.Client.Components;

namespace Crudspa.Education.Publisher.Client.Plugins.ElementType;

public partial class ActivityElementDesign : IElementDesign, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    [Inject] public IScrollService ScrollService { get; set; } = null!;
    [Inject] public IActivityElementService ActivityElementService { get; set; } = null!;

    public ActivityElement ActivityElement { get; set; } = null!;
    public ActivityElementDesignModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        ActivityElement = Element.RequireConfig<ActivityElement>();

        Model = new(false, ScrollService, ActivityElementService, ActivityElement);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    public void PrepareForSave() { }
}

public class ActivityElementDesignModel(
    Boolean isNew,
    IScrollService scrollService,
    IActivityElementService activityElementService,
    ActivityElement activityElement)
    : EditModel<ActivityElement>(isNew)
{
    public List<ActivityTypeFull> ActivityTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Named> ContentAreaNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchActivityTypes(),
            FetchContentAreaNames());

        if (activityElement.Activity is null)
        {
            var id = Guid.NewGuid();

            activityElement.ActivityId = id;

            activityElement.Activity = new()
            {
                Id = id,
                ActivityTypeId = ActivityTypes.MinBy(x => x.Name)?.Id,
                ContentAreaId = ContentAreaNames.MinBy(x => x.Name)?.Id,
            };
        }

        ActivityEditModel.ActivityTypes = ActivityTypes;
        ActivityEditModel.SetActivity(activityElement.Activity!);
    }

    public ActivityEditModel ActivityEditModel
    {
        get;
        set => SetProperty(ref field, value);
    } = new(scrollService);

    public async Task FetchActivityTypes()
    {
        var response = await activityElementService.FetchActivityTypes(new());
        if (response.Ok) ActivityTypes = response.Value.ToList();
    }

    public async Task FetchContentAreaNames()
    {
        var response = await activityElementService.FetchContentAreaNames(new());
        if (response.Ok) ContentAreaNames = response.Value.ToList();
    }
}
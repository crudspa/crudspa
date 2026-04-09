namespace Crudspa.Education.Publisher.Client.Components;

public partial class ActivityEdit : IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args)
    {
        PreviewVersion++;
        _ = InvokeAsync(StateHasChanged);
    }

    [Parameter, EditorRequired] public ActivityEditModel Model { get; set; } = null!;
    [Parameter, EditorRequired] public List<ActivityTypeFull> ActivityTypes { get; set; } = null!;
    [Parameter, EditorRequired] public List<Named> ContentAreaNames { get; set; } = null!;
    [Parameter] public Guid? PreviewPortalId { get; set; }
    [Parameter] public Boolean ReadOnly { get; set; }

    public Int32 PreviewVersion { get; set; }

    protected override Task OnInitializedAsync()
    {
        Model.PropertyChanged += HandleModelChanged;
        Model.ActivityTypes = ActivityTypes;
        Model.ContentAreaNames = ContentAreaNames;
        Model.SetSelectedType();
        Model.SetSelectedContentArea();

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }
}

public class ActivityEditModel : EditModel<Activity>
{
    private readonly IScrollService _scrollService;

    public List<ActivityTypeFull> ActivityTypes { get; set; } = [];
    public List<Named> ContentAreaNames { get; set; } = [];
    public BatchModel<ActivityChoice> ActivityChoicesModel { get; } = new();

    public ActivityEditModel(IScrollService scrollService) : base(false)
    {
        _scrollService = scrollService;
        ActivityChoicesModel.PropertyChanged += HandleEntityChanged;
    }

    public override void Dispose()
    {
        if (Entity is not null)
            Entity.PropertyChanged -= HandleEntityChanged;

        ActivityChoicesModel.PropertyChanged -= HandleEntityChanged;

        base.Dispose();
    }

    public void AddActivityChoice()
    {
        var id = Guid.NewGuid();

        ActivityChoicesModel.Entities.Add(new()
        {
            Id = id,
            ActivityId = Entity!.Id,
            IsCorrect = false,
            ColumnOrdinal = 0,
            Ordinal = ActivityChoicesModel.Entities.Count,
        });

        _scrollService.ToId(id);
    }

    public void SetActivity(Activity activity)
    {
        if (Entity is not null)
            Entity.PropertyChanged -= HandleEntityChanged;

        Entity = activity;
        Entity.PropertyChanged += HandleEntityChanged;

        ActivityChoicesModel.Entities = activity.ActivityChoices;

        SetSelectedType();
        SetSelectedContentArea();
    }

    private void HandleEntityChanged(Object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName.IsBasically(nameof(Entity.ActivityTypeId)))
            SetSelectedType();

        if (args.PropertyName.IsBasically(nameof(Entity.ContentAreaId)))
            SetSelectedContentArea();

        RaisePropertyChanged(nameof(Entity));
    }

    public void SetSelectedType()
    {
        if (Entity is null)
            return;

        SelectedActivityType = ActivityTypes.FirstOrDefault(x => x.Id.Equals(Entity.ActivityTypeId));

        if (SelectedActivityType is not null)
        {
            Entity.ActivityTypeKey = SelectedActivityType.Key;
            Entity.ActivityTypeDisplayView = SelectedActivityType.DisplayView;
            Entity.ActivityTypeCategoryName = SelectedActivityType.CategoryName;
            Entity.ActivityTypeShuffleChoices = SelectedActivityType.ShuffleChoices;
        }
    }

    public void SetSelectedContentArea()
    {
        if (Entity is null)
            return;

        SelectedContentArea = ContentAreaNames.FirstOrDefault(x => x.Id.Equals(Entity.ContentAreaId));

        if (SelectedContentArea is not null)
        {
            Entity.ContentAreaName = SelectedContentArea.Name;
            Entity.ContentAreaAppNavText = SelectedContentArea.Name;
        }
    }

    public ActivityTypeFull? SelectedActivityType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Named? SelectedContentArea
    {
        get;
        set => SetProperty(ref field, value);
    }
}
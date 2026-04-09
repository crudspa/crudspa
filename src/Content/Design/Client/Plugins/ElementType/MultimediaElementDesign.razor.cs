using HtmlEditorMarkup = Crudspa.Framework.Core.Client.Components.HtmlEditorMarkup;

namespace Crudspa.Content.Design.Client.Plugins.ElementType;

public partial class MultimediaElementDesign : IElementDesign, IDisposable
{
    private const String ElementStylesTab = "Styles";
    private const String ElementSizingTab = "Sizing";
    private const String ElementLayoutTab = "Layout";
    private const String ItemStylesTab = "Styles";
    private const String ItemSizingTab = "Sizing";
    private static MultimediaItem.MediaTypes[] MediaTypes { get; } = Enum.GetValues<MultimediaItem.MediaTypes>();

    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    [Inject] public IContainerService ContainerService { get; set; } = null!;
    [Inject] public IItemService ItemService { get; set; } = null!;
    [Inject] public IFontService FontService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public MultimediaElement MultimediaElement { get; set; } = null!;
    public BatchModel<MultimediaItemElementDesignModel> MultimediaItemsBatchModel { get; set; } = null!;
    public ModalModel AddMultimediaItemModel { get; set; } = null!;
    public BoxModel ElementBoxModel { get; private set; } = null!;
    public ItemModel ElementItemModel { get; private set; } = null!;
    public String ActiveElementSettingsTab { get; private set; } = ElementStylesTab;

    public String SettingsDescription => BuildSettingsDescription(ElementBoxModel.Description, ElementItemModel.Description, ContainerModel?.Description);

    public ContainerModel? ContainerModel { get; private set; }

    protected override async Task OnInitializedAsync()
    {
        AddMultimediaItemModel = new(ScrollService);

        MultimediaElement = Element.RequireConfig<MultimediaElement>();

        ElementBoxModel = new(ScrollService, Element.Box);
        ElementBoxModel.PropertyChanged += HandleModelChanged;

        ElementItemModel = new(ScrollService, ItemService, Element.Item);
        ElementItemModel.PropertyChanged += HandleModelChanged;
        await ElementItemModel.Initialize();

        MultimediaItemsBatchModel = new()
        {
            Entities = MultimediaElement.MultimediaItems
                .Select(x => new MultimediaItemElementDesignModel(x, FontService, ScrollService, ItemService))
                .ToObservable(),
        };

        foreach (var builderModel in MultimediaItemsBatchModel.Entities)
            await builderModel.Initialize();

        MultimediaItemsBatchModel.PropertyChanged += HandleModelChanged;

        ContainerModel = new(ScrollService, ContainerService, MultimediaElement.Container);
        ContainerModel.PropertyChanged += HandleModelChanged;

        await ContainerModel.Initialize();
    }

    public void Dispose()
    {
        MultimediaItemsBatchModel.PropertyChanged -= HandleModelChanged;

        ElementBoxModel.PropertyChanged -= HandleModelChanged;
        ElementItemModel.PropertyChanged -= HandleModelChanged;

        ElementBoxModel.Dispose();
        ElementItemModel.Dispose();

        if (ContainerModel is not null)
        {
            ContainerModel.PropertyChanged -= HandleModelChanged;
            ContainerModel.Dispose();
        }

        MultimediaItemsBatchModel.Dispose();
    }

    public void PrepareForSave()
    {
        foreach (var multimediaItem in MultimediaItemsBatchModel.Entities.Select(x => x.MultimediaItem)
                     .Where(x => x.MediaTypeIndex == MultimediaItem.MediaTypes.Text))
            multimediaItem.Text = HtmlEditorMarkup.NormalizeForStorage(multimediaItem.Text);

        MultimediaElement.MultimediaItems = MultimediaItemsBatchModel.Entities.Select(x => x.MultimediaItem).ToObservable();
    }

    public async Task AddMultimediaItem(MultimediaItem.MediaTypes mediaType)
    {
        if (ReadOnly)
            return;

        var id = Guid.NewGuid();

        var multimediaItem = new MultimediaItem
        {
            Id = id,
            MultimediaElementId = MultimediaElement.Id,
            MediaTypeIndex = mediaType,
            Item = new()
            {
                BasisId = BasisIds.Auto,
                Grow = "0",
                Shrink = "1",
                AlignSelfId = AlignSelfIds.Auto,
            },
            Ordinal = MultimediaItemsBatchModel.Entities.Count,
        };

        var builderModel = new MultimediaItemElementDesignModel(multimediaItem, FontService, ScrollService, ItemService);

        MultimediaItemsBatchModel.Entities.Add(builderModel);

        await builderModel.Initialize();

        await AddMultimediaItemModel.Hide();

        await ScrollService.ToId(id);
    }

    private static String GetMediaTypeIconClass(MultimediaItem.MediaTypes mediaType)
    {
        return mediaType switch
        {
            MultimediaItem.MediaTypes.Audio => "c-icon-volume-high",
            MultimediaItem.MediaTypes.Button => "c-icon-hand-click",
            MultimediaItem.MediaTypes.Image => "c-icon-photo",
            MultimediaItem.MediaTypes.Text => "c-icon-article",
            MultimediaItem.MediaTypes.Video => "c-icon-video",
            _ => "c-icon-shape",
        };
    }

    private static String GetMediaTypeName(MultimediaItem.MediaTypes mediaType)
    {
        return mediaType switch
        {
            MultimediaItem.MediaTypes.Audio => "Audio",
            MultimediaItem.MediaTypes.Button => "Button",
            MultimediaItem.MediaTypes.Image => "Image",
            MultimediaItem.MediaTypes.Text => "Text",
            MultimediaItem.MediaTypes.Video => "Video",
            _ => "Media",
        };
    }

    private Task OpenElementSettings()
    {
        if (ReadOnly)
            return Task.CompletedTask;

        ActiveElementSettingsTab = ElementStylesTab;
        ElementItemModel.RefreshOverrideStates();
        return ElementBoxModel.Show();
    }

    private Task HideElementSettings()
    {
        ElementItemModel.NormalizeOverrideValues();
        return ElementBoxModel.Hide();
    }

    private void SetElementSettingsTab(String tab) => ActiveElementSettingsTab = tab;

    private Task OpenItemSettings(MultimediaItemElementDesignModel model)
    {
        if (ReadOnly)
            return Task.CompletedTask;

        model.ActiveTab = ItemStylesTab;
        model.ItemModel.RefreshOverrideStates();
        return model.BoxModel.Show();
    }

    private Task HideItemSettings(MultimediaItemElementDesignModel model)
    {
        model.ItemModel.NormalizeOverrideValues();
        return model.BoxModel.Hide();
    }

    private static void SetItemSettingsTab(MultimediaItemElementDesignModel model, String tab) => model.ActiveTab = tab;

    private static String GetItemSettingsDescription(MultimediaItemElementDesignModel model) =>
        BuildSettingsDescription(model.BoxModel.Description, model.ItemModel.Description);

    private static String BuildSettingsDescription(params String?[] descriptions)
    {
        var values = descriptions
            .Where(x => x.HasSomething())
            .SelectMany(x => x!.Split('|', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
            .Where(x => x.HasSomething() && !x.IsBasically("None"))
            .ToList();

        return values.HasItems() ? String.Join(" | ", values) : "Default";
    }
}

public class MultimediaItemElementDesignModel : Observable, IDisposable, IOrderable, INamed
{
    private void HandleBoxModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(BoxModel));
    private void HandleItemModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ItemModel));

    public String Name => "Multimedia";

    public MultimediaItemElementDesignModel(MultimediaItem multimediaItem,
        IFontService fontService,
        IScrollService scrollService,
        IItemService itemService)
    {
        MultimediaItem = multimediaItem;

        BoxModel = new(scrollService, MultimediaItem.Box);
        BoxModel.PropertyChanged += HandleBoxModelChanged;

        ItemModel = new(scrollService, itemService, MultimediaItem.Item);
        ItemModel.PropertyChanged += HandleItemModelChanged;

        ButtonModel = new(fontService, scrollService, MultimediaItem.Button);
    }

    public void Dispose()
    {
        BoxModel.PropertyChanged -= HandleBoxModelChanged;
        ItemModel.PropertyChanged -= HandleItemModelChanged;

        BoxModel.Dispose();
        ItemModel.Dispose();
    }

    public async Task Initialize()
    {
        await ItemModel.Initialize();
        await ButtonModel.Initialize();
    }

    public MultimediaItem MultimediaItem { get; }
    public BoxModel BoxModel { get; }
    public ItemModel ItemModel { get; }
    public ButtonEditModel ButtonModel { get; }
    public String ActiveTab { get; set; } = "Styles";

    public Guid? Id
    {
        get => MultimediaItem.Id;
        set => MultimediaItem.Id = value;
    }

    public Int32? Ordinal
    {
        get => MultimediaItem.Ordinal;
        set => MultimediaItem.Ordinal = value;
    }
}
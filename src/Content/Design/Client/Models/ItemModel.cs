namespace Crudspa.Content.Design.Client.Models;

public class ItemModel : ModalModel
{
    private void HandleItemChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(Item));
    public void SetActiveTab(String key) => ActiveTab = key;

    private readonly IItemService _itemService;

    private Item _item;

    public ItemModel(IScrollService scrollService, IItemService itemService, Item item)
        : base(scrollService)
    {
        _itemService = itemService;

        _item = item;
        _item.PropertyChanged += HandleItemChanged;
    }

    public override void Dispose()
    {
        _item.PropertyChanged -= HandleItemChanged;
        base.Dispose();
    }

    public override Task Show()
    {
        RefreshOverrideStates();

        return base.Show();
    }

    public override Task Hide()
    {
        NormalizeOverrideValues();

        return base.Hide();
    }

    public void RefreshOverrideStates()
    {
        if (_item.MaxWidth.HasNothing())
            _item.MaxWidth = null;

        if (_item.MinWidth.HasNothing())
            _item.MinWidth = null;

        if (_item.Width.HasNothing())
            _item.Width = null;

        MaxWidthState = _item.MaxWidth.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        MinWidthState = _item.MinWidth.HasSomething() ? OverrideState.Custom : OverrideState.Default;
        WidthState = _item.Width.HasSomething() ? OverrideState.Custom : OverrideState.Default;
    }

    public void NormalizeOverrideValues()
    {
        if (MaxWidthState == OverrideState.Default || _item.MaxWidth.HasNothing())
            _item.MaxWidth = null;

        if (MinWidthState == OverrideState.Default || _item.MinWidth.HasNothing())
            _item.MinWidth = null;

        if (WidthState == OverrideState.Default || _item.Width.HasNothing())
            _item.Width = null;
    }

    public Item Item
    {
        get => _item;
        set => SetProperty(ref _item, value);
    }

    public List<Orderable> BasisNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public List<Orderable> AlignSelfNames
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public String ActiveTab
    {
        get;
        set => SetProperty(ref field, value);
    } = "Flex";

    public OverrideState MaxWidthState
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value == OverrideState.Default)
                _item.MaxWidth = null;
        }
    }

    public OverrideState MinWidthState
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value == OverrideState.Default)
                _item.MinWidth = null;
        }
    }

    public OverrideState WidthState
    {
        get;
        set
        {
            if (SetProperty(ref field, value) && value == OverrideState.Default)
                _item.Width = null;
        }
    }

    public async Task Initialize()
    {
        await WithMany("Initializing...",
            FetchBasisNames(),
            FetchAlignSelfNames());
    }

    public async Task FetchBasisNames()
    {
        var response = await WithAlerts(() => _itemService.FetchBasisNames(new()), false);
        if (response.Ok) BasisNames = response.Value.ToList();
    }

    public async Task FetchAlignSelfNames()
    {
        var response = await WithAlerts(() => _itemService.FetchAlignSelfNames(new()), false);
        if (response.Ok) AlignSelfNames = response.Value.ToList();
    }

    public String Description
    {
        get
        {
            List<String> values = [];

            if (Item.BasisId is not null)
                values.Add($"Basis: {BasisNames.FirstOrDefault(x => x.Id.Equals(Item.BasisId))?.Name}");

            if (!Item.BasisId.Equals(BasisIds.Auto) && Item.BasisAmount.HasSomething())
                values.Add($"Amount: {Item.BasisAmount}");

            if (Item.Grow.HasSomething())
                values.Add($"Grow: {Item.Grow}");

            if (Item.Shrink.HasSomething())
                values.Add($"Shrink: {Item.Shrink}");

            if (Item.AlignSelfId is not null)
                values.Add($"Align Self: {AlignSelfNames.FirstOrDefault(x => x.Id.Equals(Item.AlignSelfId))?.Name}");

            if (Item.MaxWidth.HasSomething())
                values.Add($"Max Width: {Item.MaxWidth}");

            if (Item.MinWidth.HasSomething())
                values.Add($"Min Width: {Item.MinWidth}");

            if (Item.Width.HasSomething())
                values.Add($"Width: {Item.Width}");

            return values.HasItems() ? String.Join(" | ", values) : "None";
        }
    }
}
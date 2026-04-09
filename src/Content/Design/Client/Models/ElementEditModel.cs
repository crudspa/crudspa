using Crudspa.Content.Design.Client.Plugins;

namespace Crudspa.Content.Design.Client.Models;

public class ElementEditModel : Observable, IDisposable, IOrderable, INamed
{
    private void HandleBoxModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(BoxModel));
    private void HandleItemModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(ItemModel));

    private readonly List<ElementType> _elementTypes;

    public String? Name => Element.ElementType?.Name;

    public ElementEditModel(SectionElement element, IScrollService scrollService, IItemService itemService, List<ElementType> elementTypes)
    {
        Element = element;

        _elementTypes = elementTypes;
        SetSelectedType();

        Element.PropertyChanged += HandleElementChanged;

        BoxModel = new(scrollService, Element.Box);
        BoxModel.PropertyChanged += HandleBoxModelChanged;

        ItemModel = new(scrollService, itemService, Element.Item);
        ItemModel.PropertyChanged += HandleItemModelChanged;
    }

    public async Task Initialize()
    {
        await ItemModel.Initialize();
    }

    public void Dispose()
    {
        Element.PropertyChanged -= HandleElementChanged;
        BoxModel.PropertyChanged -= HandleBoxModelChanged;
        ItemModel.PropertyChanged -= HandleItemModelChanged;
        BoxModel.Dispose();
        ItemModel.Dispose();
    }

    private void HandleElementChanged(Object? sender, PropertyChangedEventArgs args)
    {
        if (args.PropertyName.IsBasically(nameof(Element.TypeId)))
            SetSelectedType();

        RaisePropertyChanged(nameof(Element));
    }

    public ElementDesignPlugin DesignComponent { get; set; } = null!;
    public SectionElement Element { get; }
    public BoxModel BoxModel { get; }
    public ItemModel ItemModel { get; }

    public Guid? Id
    {
        get => Element.Id;
        set => Element.Id = value;
    }

    public Int32? Ordinal
    {
        get => Element.Ordinal;
        set => Element.Ordinal = value;
    }

    public void SetSelectedType()
    {
        SelectedElementType = _elementTypes.FirstOrDefault(x => x.Id.Equals(Element.TypeId));
    }

    public ElementType? SelectedElementType
    {
        get;
        set => SetProperty(ref field, value);
    }
}
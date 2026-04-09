namespace Crudspa.Content.Design.Client.Models;

public class ButtonEditModel : Observable, IDisposable
{
    private void HandleBoxModelChanged(Object? sender, PropertyChangedEventArgs args) => RaisePropertyChanged(nameof(BoxModel));

    private readonly IFontService _fontService;
    private Button _button;

    public ButtonEditModel(IFontService fontService, IScrollService scrollService, Button button)
    {
        _fontService = fontService;
        _button = button;

        BoxModel = new(scrollService, button.Box);
        BoxModel.PropertyChanged += HandleBoxModelChanged;
    }

    public void Dispose()
    {
        BoxModel.PropertyChanged -= HandleBoxModelChanged;
        BoxModel.Dispose();
    }

    public async Task Initialize()
    {
        await FetchIcons();
    }

    public Button Button
    {
        get => _button;
        set => SetProperty(ref _button, value);
    }

    public BoxModel BoxModel { get; }

    public List<IconFull> Icons
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public async Task FetchIcons()
    {
        var response = await _fontService.FetchIcons(new());
        if (response.Ok) Icons = response.Value.ToList();
    }
}
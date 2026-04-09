using Crudspa.Content.Display.Client.Contracts.Events;

namespace Crudspa.Content.Display.Client.Plugins.ElementType;

public partial class NoteElementDisplay : IElementDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public ElementDisplayModel ElementModel { get; set; } = null!;

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public IElementProgressService ElementProgressService { get; set; } = null!;
    [Inject] public IMediaPlayService MediaPlayService { get; set; } = null!;

    public NoteElementDisplayModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var noteElement = ElementModel.RequireConfig<NoteElement>();

        Model = new(ElementModel, noteElement);
        Model.PropertyChanged += HandleModelChanged;

        await Model.ElementModel.InitializeProgress();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
    }

    private async Task HandleClicked()
    {
        await EventBus.Publish(new AddNotepage { Note = Model.NoteElement });
        await Model.ElementModel.AddElementCompleted();
    }
}

public class NoteElementDisplayModel(ElementDisplayModel elementModel, NoteElement noteElement) : Observable
{
    public ElementDisplayModel ElementModel { get; } = elementModel;

    public NoteElement NoteElement
    {
        get;
        set => SetProperty(ref field, value);
    } = noteElement;
}
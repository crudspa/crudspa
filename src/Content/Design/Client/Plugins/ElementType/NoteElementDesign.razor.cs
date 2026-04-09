namespace Crudspa.Content.Design.Client.Plugins.ElementType;

public partial class NoteElementDesign : IElementDesign, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    [Inject] public IScrollService ScrollService { get; set; } = null!;

    public NoteElement Note { get; set; } = null!;
    public BatchModel<NoteImage> ImageChoicesModel { get; set; } = null!;

    protected override Task OnInitializedAsync()
    {
        Note = Element.RequireConfig<NoteElement>();

        ImageChoicesModel = new() { Entities = Note.NoteImages };
        ImageChoicesModel.PropertyChanged += HandleModelChanged;

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        ImageChoicesModel.PropertyChanged -= HandleModelChanged;
        ImageChoicesModel.Dispose();
    }

    public void PrepareForSave() { }

    private async Task AddImageChoice()
    {
        if (ReadOnly)
            return;

        var id = Guid.NewGuid();

        ImageChoicesModel.Entities.Add(new()
        {
            Id = id,
            NoteId = Note.Id,
            Ordinal = ImageChoicesModel.Entities.Count,
        });

        await ScrollService.ToId(id);
    }
}
using Crudspa.Content.Display.Client.Contracts.Events;
using Crudspa.Framework.Core.Client.Components;

namespace Crudspa.Content.Display.Client.Components;

public partial class RootModalsContent : IDisposable, IHandle<OpenNotebook>, IHandle<AddNotepage>
{
    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INotebookRunService NotebookRunService { get; set; } = null!;
    [Inject] public IScrollService ScrollService { get; set; } = null!;

    [Parameter, EditorRequired] public ContentPortal PortalRun { get; set; } = null!;
    [Parameter] public ImageViewerModel? ImageViewerModel { get; set; }
    [Parameter] public PdfViewerModel? PdfViewerModel { get; set; }
    [Parameter] public MyAchievementsModel? MyAchievementsModel { get; set; }
    [Parameter] public AchievementModel? AchievementModel { get; set; }
    public NotebookModel? NotebookModel { get; set; }

    protected override Task OnInitializedAsync()
    {
        NotebookModel = new(ScrollService, NotebookRunService);

        EventBus.Subscribe(this);

        return Task.CompletedTask;
    }

    public void Dispose()
    {
        EventBus.Unsubscribe(this);
    }

    public async Task Handle(OpenNotebook payload)
    {
        if (NotebookModel is not null)
            await NotebookModel.Show();
    }

    public async Task Handle(AddNotepage payload)
    {
        if (NotebookModel is not null)
            await NotebookModel.Show(payload.Note);
    }
}
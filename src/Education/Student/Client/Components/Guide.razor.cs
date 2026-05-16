namespace Crudspa.Education.Student.Client.Components;

public partial class Guide : IDisposable
{
    private GuideModel? _subscribedModel;

    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public GuideModel? Model { get; set; }
    [Parameter] public GuideBinder? GuideBinder { get; set; }
    [Parameter] public Page? Page { get; set; }
    [Parameter] public Boolean ShowGuideFields { get; set; } = true;
    [Parameter] public Boolean ShowNotebookButton { get; set; }
    [Parameter] public Boolean AutoPlay { get; set; } = true;

    [Inject] public IEventBus EventBus { get; set; } = null!;

    public GuideModel? RenderModel { get; private set; }
    public Boolean RenderShowGuideFields { get; private set; }
    public Boolean RenderShowNotebookButton { get; private set; }

    protected override void OnParametersSet()
    {
        if (Model is not null)
        {
            Subscribe(Model);
            RenderModel = Model;
            RenderShowGuideFields = ShowGuideFields;
            RenderShowNotebookButton = ShowNotebookButton;
            return;
        }

        Subscribe(null);

        var guidePage = Page?.Id is Guid pageId
            ? GuideBinder?.Pages.FirstOrDefault(x => x.PageId.Equals(pageId))
            : null;

        if (guidePage is null || guidePage.ShowGuide != true && guidePage.ShowNotebook != true)
        {
            RenderModel = null;
            RenderShowGuideFields = false;
            RenderShowNotebookButton = false;
            return;
        }

        RenderModel = new()
        {
            Image = GuideBinder?.GuideImage,
            Text = guidePage.GuideText,
            Audio = guidePage.GuideAudioFile.Id.HasValue ? guidePage.GuideAudioFile : guidePage.GuideAudio,
        };
        RenderShowGuideFields = guidePage.ShowGuide == true;
        RenderShowNotebookButton = guidePage.ShowNotebook == true;
    }

    public void Dispose()
    {
        Subscribe(null);
    }

    private void Subscribe(GuideModel? model)
    {
        if (ReferenceEquals(_subscribedModel, model))
            return;

        if (_subscribedModel is not null)
            _subscribedModel.PropertyChanged -= HandleModelChanged;

        _subscribedModel = model;

        if (_subscribedModel is not null)
            _subscribedModel.PropertyChanged += HandleModelChanged;
    }
}

public class GuideModel : Observable
{
    public ImageFile? Image
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile? Audio
    {
        get;
        set => SetProperty(ref field, value);
    }
}
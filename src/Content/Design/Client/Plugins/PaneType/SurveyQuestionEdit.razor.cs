using Crudspa.Content.Design.Client.Plugins;
using AnswerTypeData = Crudspa.Content.Display.Shared.Contracts.Data.AnswerType;
using HtmlEditorMarkup = Crudspa.Framework.Core.Client.Components.HtmlEditorMarkup;

namespace Crudspa.Content.Design.Client.Plugins.PaneType;

public partial class SurveyQuestionEdit : IPaneDisplay, IDisposable
{
    private void HandleModelChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public String? Path { get; set; }
    [Parameter] public Guid? Id { get; set; }
    [Parameter] public Boolean IsNew { get; set; }
    [Parameter] public String? ConfigJson { get; set; }

    [Inject] public IEventBus EventBus { get; set; } = null!;
    [Inject] public INavigator Navigator { get; set; } = null!;
    [Inject] public ISectionService SectionService { get; set; } = null!;
    [Inject] public ISurveyService SurveyService { get; set; } = null!;

    public SurveyQuestionEditModel Model { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        var partId = Path!.Id("survey-part");

        Model = new(Path, Id, IsNew, partId, EventBus, Navigator, SectionService, SurveyService);
        Model.PropertyChanged += HandleModelChanged;

        await Model.Initialize();
    }

    public void Dispose()
    {
        Model.PropertyChanged -= HandleModelChanged;
        Model.Dispose();
    }

    private async Task HandleCancelClicked()
    {
        if (Model.IsNew)
            Navigator.Close(Path);
        else
            await Model.Refresh();
    }
}

public class SurveyQuestionEditModel : EditModel<SurveyQuestion>, IHandle<SurveyQuestionSaved>, IHandle<SurveyQuestionRemoved>
{
    private readonly String? _path;
    private readonly Guid? _id;
    private readonly Guid? _partId;
    private readonly INavigator _navigator;
    private readonly ISectionService _sectionService;
    private readonly ISurveyService _surveyService;

    public SurveyQuestionEditModel(String? path, Guid? id, Boolean isNew, Guid? partId,
        IEventBus eventBus,
        INavigator navigator,
        ISectionService sectionService,
        ISurveyService surveyService) : base(isNew)
    {
        _path = path;
        _id = id;
        _partId = partId;
        _navigator = navigator;
        _sectionService = sectionService;
        _surveyService = surveyService;

        eventBus.Subscribe(this);
    }

    public List<AnswerTypeData> AnswerTypes
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public AnswerDesignPlugin? AnswerDesign { get; set; }

    public AnswerTypeData? SelectedAnswerType => AnswerTypes.FirstOrDefault(x => x.Id == Entity?.Question.AnswerTypeId);

    public async Task Handle(SurveyQuestionSaved payload)
    {
        if (payload.Id.Equals(_id))
            await Refresh();
    }

    public Task Handle(SurveyQuestionRemoved payload)
    {
        if (payload.Id.Equals(_id))
            _navigator.Close(_path);

        return Task.CompletedTask;
    }

    public async Task Initialize()
    {
        await FetchAnswerTypes();
        await Refresh();
    }

    public async Task Refresh()
    {
        if (IsNew)
        {
            ReadOnly = false;

            SetQuestion(new()
            {
                PartId = _partId,
                Question = new()
                {
                    AnswerTypeId = AnswerTypeIds.Text,
                    TextAnswer = new(),
                },
            });
        }
        else
        {
            ReadOnly = true;

            var response = await WithWaiting("Fetching...", () =>
                _surveyService.FetchQuestion(new(new() { Id = _id })));

            if (response.Ok)
                SetQuestion(response.Value);
        }
    }

    public async Task Save()
    {
        PrepareForSave();

        if (IsNew)
        {
            var response = await WithWaiting("Adding...", () => _surveyService.AddQuestion(new(Entity!)));

            if (response.Ok)
            {
                _navigator.GoTo($"{_path.Parent()}/survey-question-{response.Value.Id:D}");
                _navigator.Close(_path);
            }
        }
        else
        {
            var response = await WithWaiting("Saving...", () => _surveyService.SaveQuestion(new(Entity!)));

            if (response.Ok)
                ReadOnly = true;
        }
    }

    private async Task FetchAnswerTypes()
    {
        var response = await WithAlerts(() => _sectionService.FetchAnswerTypes(new()), false);
        if (response.Ok) AnswerTypes = response.Value.ToList();
    }

    private void PrepareForSave()
    {
        Entity!.Question.Text = HtmlEditorMarkup.NormalizeForStorage(Entity.Question.Text);
        Entity.Question.EnsureAnswer();

        if (AnswerDesign?.Instance is IAnswerDesign answerDesign)
            answerDesign.PrepareForSave();
    }

    private void SetQuestion(SurveyQuestion? question)
    {
        Entity = question;
        Entity?.Question.EnsureAnswer();
        _navigator.UpdateTitle(_path, "Question");
    }
}
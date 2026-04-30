using Crudspa.Content.Design.Client.Plugins;
using AnswerTypeData = Crudspa.Content.Display.Shared.Contracts.Data.AnswerType;
using HtmlEditorMarkup = Crudspa.Framework.Core.Client.Components.HtmlEditorMarkup;

namespace Crudspa.Content.Design.Client.Plugins.ElementType;

public partial class QuestionElementDesign : IElementDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public SectionElement Element { get; set; } = null!;

    [Inject] public ISectionService SectionService { get; set; } = null!;

    public QuestionElement QuestionElement { get; set; } = null!;
    public List<AnswerTypeData> AnswerTypes { get; set; } = [];
    public AnswerDesignPlugin AnswerDesign { get; set; } = null!;

    public AnswerTypeData? SelectedAnswerType => AnswerTypes.FirstOrDefault(x => x.Id == QuestionElement.Question.AnswerTypeId);

    protected override async Task OnInitializedAsync()
    {
        QuestionElement = Element.RequireConfig<QuestionElement>();
        QuestionElement.Question.EnsureAnswer();

        var response = await SectionService.FetchAnswerTypes(new());
        if (response.Ok)
            AnswerTypes = response.Value.ToList();
    }

    public void PrepareForSave()
    {
        QuestionElement.Question.Text = HtmlEditorMarkup.NormalizeForStorage(QuestionElement.Question.Text);
        QuestionElement.Question.EnsureAnswer();

        if (AnswerDesign.Instance is IAnswerDesign answerDesign)
            answerDesign.PrepareForSave();
    }
}
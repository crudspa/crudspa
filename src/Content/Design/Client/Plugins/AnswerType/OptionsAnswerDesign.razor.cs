using HtmlEditorMarkup = Crudspa.Framework.Core.Client.Components.HtmlEditorMarkup;

namespace Crudspa.Content.Design.Client.Plugins.AnswerType;

public partial class OptionsAnswerDesign : IAnswerDesign, IDisposable
{
    private void HandleChoicesChanged(Object? sender, PropertyChangedEventArgs args) => InvokeAsync(StateHasChanged);

    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Question Question { get; set; } = null!;

    public OptionsAnswer Answer => Question.OptionsAnswer!;
    public BatchModel<OptionsAnswerChoice> ChoicesModel { get; set; } = new();

    protected override void OnInitialized()
    {
        Question.EnsureAnswer();
        ChoicesModel.SetEntities(Answer.Choices);
        ChoicesModel.PropertyChanged += HandleChoicesChanged;
    }

    public void Dispose() => ChoicesModel.PropertyChanged -= HandleChoicesChanged;

    public Task AddChoice()
    {
        ChoicesModel.AddEntity(new()
        {
            Id = Guid.NewGuid(),
            OptionsAnswerId = Answer.Id,
            Ordinal = ChoicesModel.Entities.Count,
        });

        return Task.CompletedTask;
    }

    public void PrepareForSave()
    {
        foreach (var choice in ChoicesModel.Entities)
            choice.Text = HtmlEditorMarkup.NormalizeForStorage(choice.Text);

        ChoicesModel.Entities.EnsureOrder();
        Answer.Choices = ChoicesModel.Entities.ToObservable();
    }
}
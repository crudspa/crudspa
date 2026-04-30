namespace Crudspa.Content.Display.Client.Plugins.AnswerType;

public partial class OptionsAnswerDisplay : IAnswerDisplay
{
    private String? _randomChoiceKey;
    private IList<OptionsAnswerChoice>? _randomChoices;

    [Parameter] public QuestionDisplayModel Model { get; set; } = null!;

    public OptionsAnswer? Answer => Model.Question.OptionsAnswer;
    public String GroupName => $"question-{Model.Question.Id:N}";

    public IList<OptionsAnswerChoice> Choices
    {
        get
        {
            var choices = Answer?.Choices.OrderBy(x => x.Ordinal).ToList() ?? [];
            if (Answer?.Ordering != OptionsAnswer.Orderings.Random)
                return choices;

            var key = String.Join("|", choices.Select(x => x.Id?.ToString("N") ?? x.Text ?? String.Empty));
            if (_randomChoices is null || !_randomChoiceKey.IsBasically(key))
            {
                _randomChoiceKey = key;
                _randomChoices = choices.OrderBy(_ => Guid.NewGuid()).ToList();
            }

            return _randomChoices;
        }
    }

    private void SelectSingle(OptionsAnswerChoice choice)
    {
        Model.Reply.OtherBoolValue = false;
        Model.Reply.OtherTextValue = null;
        Model.SetSingleChoice(choice);
    }

    private void Toggle(OptionsAnswerChoice choice, ChangeEventArgs args) =>
        Model.ToggleChoice(choice, args.Value is Boolean value && value);

    private void SelectOther()
    {
        Model.Reply.AnswerChoices.Clear();
        Model.Reply.OtherBoolValue = true;
    }

    private void ToggleOther(ChangeEventArgs args)
    {
        Model.Reply.OtherBoolValue = args.Value is Boolean value && value;

        if (Model.Reply.OtherBoolValue != true)
            Model.Reply.OtherTextValue = null;
    }
}
namespace Crudspa.Content.Display.Client.Plugins.AnswerType;

public partial class BooleanAnswerDisplay : IAnswerDisplay
{
    [Parameter] public QuestionDisplayModel Model { get; set; } = null!;

    public BooleanAnswer? Answer => Model.Question.BooleanAnswer;
    public String GroupName => $"question-{Model.Question.Id:N}";

    private void ToggleCheckbox(ChangeEventArgs args) =>
        Model.SetBoolean(args.Value is Boolean value && value);
}
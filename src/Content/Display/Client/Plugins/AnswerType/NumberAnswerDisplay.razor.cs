namespace Crudspa.Content.Display.Client.Plugins.AnswerType;

public partial class NumberAnswerDisplay : IAnswerDisplay
{
    [Parameter] public QuestionDisplayModel Model { get; set; } = null!;

    public NumberAnswer? Answer => Model.Question.NumberAnswer;
}
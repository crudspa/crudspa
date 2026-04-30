namespace Crudspa.Content.Display.Client.Plugins.AnswerType;

public partial class TextAnswerDisplay : IAnswerDisplay
{
    [Parameter] public QuestionDisplayModel Model { get; set; } = null!;

    public TextAnswer? Answer => Model.Question.TextAnswer;
}
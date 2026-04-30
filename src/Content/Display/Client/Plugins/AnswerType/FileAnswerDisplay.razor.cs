namespace Crudspa.Content.Display.Client.Plugins.AnswerType;

public partial class FileAnswerDisplay : IAnswerDisplay
{
    [Parameter] public QuestionDisplayModel Model { get; set; } = null!;

    public FileAnswer? Answer => Model.Question.FileAnswer;
}
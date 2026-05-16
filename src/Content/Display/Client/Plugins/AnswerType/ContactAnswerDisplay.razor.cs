namespace Crudspa.Content.Display.Client.Plugins.AnswerType;

public partial class ContactAnswerDisplay : IAnswerDisplay
{
    [Parameter] public QuestionDisplayModel Model { get; set; } = null!;

    public ContactAnswer? Answer => Model.Question.ContactAnswer;
}
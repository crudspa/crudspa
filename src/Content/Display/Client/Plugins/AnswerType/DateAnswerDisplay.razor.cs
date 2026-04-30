namespace Crudspa.Content.Display.Client.Plugins.AnswerType;

public partial class DateAnswerDisplay : IAnswerDisplay
{
    [Parameter] public QuestionDisplayModel Model { get; set; } = null!;

    public DateAnswer? Answer => Model.Question.DateAnswer;

    public String? TimeText
    {
        get => Model.Reply.TimeValue?.ToString("HH:mm");
        set => Model.Reply.TimeValue = TimeOnly.TryParse(value, out var parsed) ? parsed : null;
    }
}
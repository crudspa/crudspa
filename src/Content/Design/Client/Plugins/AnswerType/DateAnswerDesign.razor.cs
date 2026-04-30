namespace Crudspa.Content.Design.Client.Plugins.AnswerType;

public partial class DateAnswerDesign : IAnswerDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Question Question { get; set; } = null!;

    public DateAnswer Answer => Question.DateAnswer!;

    public String? TimeMinText
    {
        get => Answer.TimeMin?.ToString("HH:mm");
        set => Answer.TimeMin = TimeOnly.TryParse(value, out var parsed) ? parsed : null;
    }

    public String? TimeMaxText
    {
        get => Answer.TimeMax?.ToString("HH:mm");
        set => Answer.TimeMax = TimeOnly.TryParse(value, out var parsed) ? parsed : null;
    }

    protected override void OnInitialized() => Question.EnsureAnswer();

    public void PrepareForSave() { }
}
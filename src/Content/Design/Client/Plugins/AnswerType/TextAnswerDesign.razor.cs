namespace Crudspa.Content.Design.Client.Plugins.AnswerType;

public partial class TextAnswerDesign : IAnswerDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Question Question { get; set; } = null!;

    public TextAnswer Answer => Question.TextAnswer!;

    protected override void OnInitialized() => Question.EnsureAnswer();

    public void PrepareForSave() { }
}
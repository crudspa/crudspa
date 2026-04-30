namespace Crudspa.Content.Design.Client.Plugins.AnswerType;

public partial class NumberAnswerDesign : IAnswerDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Question Question { get; set; } = null!;

    public NumberAnswer Answer => Question.NumberAnswer!;

    protected override void OnInitialized() => Question.EnsureAnswer();

    public void PrepareForSave() { }
}
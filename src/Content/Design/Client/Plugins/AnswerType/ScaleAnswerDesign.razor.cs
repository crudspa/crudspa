namespace Crudspa.Content.Design.Client.Plugins.AnswerType;

public partial class ScaleAnswerDesign : IAnswerDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Question Question { get; set; } = null!;

    public ScaleAnswer Answer => Question.ScaleAnswer!;

    protected override void OnInitialized() => Question.EnsureAnswer();

    public void PrepareForSave() { }
}
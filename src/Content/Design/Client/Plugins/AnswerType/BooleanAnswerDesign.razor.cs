namespace Crudspa.Content.Design.Client.Plugins.AnswerType;

public partial class BooleanAnswerDesign : IAnswerDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Question Question { get; set; } = null!;

    public BooleanAnswer Answer => Question.BooleanAnswer!;

    protected override void OnInitialized() => Question.EnsureAnswer();

    public void PrepareForSave() { }
}
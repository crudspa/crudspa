namespace Crudspa.Content.Design.Client.Plugins.AnswerType;

public partial class FileAnswerDesign : IAnswerDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Question Question { get; set; } = null!;

    public FileAnswer Answer => Question.FileAnswer!;

    protected override void OnInitialized() => Question.EnsureAnswer();

    public void PrepareForSave() { }
}
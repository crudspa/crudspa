namespace Crudspa.Content.Design.Client.Plugins.AnswerType;

public partial class ContactAnswerDesign : IAnswerDesign
{
    [Parameter] public Boolean ReadOnly { get; set; }
    [Parameter] public Question Question { get; set; } = null!;

    public ContactAnswer Answer => Question.ContactAnswer!;

    public ContactAnswer.Kinds Kind
    {
        get => Answer.Kind;
        set => Answer.Kind = value;
    }

    protected override void OnInitialized() => Question.EnsureAnswer();

    public void PrepareForSave() { }
}
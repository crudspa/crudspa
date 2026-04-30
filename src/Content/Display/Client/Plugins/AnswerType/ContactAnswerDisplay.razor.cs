namespace Crudspa.Content.Display.Client.Plugins.AnswerType;

public partial class ContactAnswerDisplay : IAnswerDisplay
{
    [Parameter] public QuestionDisplayModel Model { get; set; } = null!;

    public ContactAnswer? Answer => Model.Question.ContactAnswer;

    public String Label => Answer?.Label.HasSomething() == true
        ? Answer.Label!
        : Answer?.Kind switch
        {
            ContactAnswer.Kinds.Email => "Email",
            ContactAnswer.Kinds.Postal => "Address",
            ContactAnswer.Kinds.Phone => "Phone",
            ContactAnswer.Kinds.Signature => "Signature",
            _ => "Response",
        };
}
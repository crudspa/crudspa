namespace Crudspa.Content.Display.Client.Components;

public partial class QuestionDisplay
{
    [Parameter] public QuestionDisplayModel? Model { get; set; }

    public String QuestionClass
    {
        get
        {
            if (Model?.Question.Text.HasSomething() == true)
                return "c-question with-question-text";

            return "c-question without-question-text";
        }
    }
}
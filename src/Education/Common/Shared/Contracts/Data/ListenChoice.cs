namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ListenChoice : Observable, IValidates, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ListenQuestionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ListenQuestionText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? IsCorrect
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public AudioFile AudioFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AssessmentChoiceStates State { get; set; }

    public String StateClass
    {
        get
        {
            switch (State)
            {
                case AssessmentChoiceStates.Selected:
                    return "selected";
                case AssessmentChoiceStates.Invalid:
                    return "invalid";
                case AssessmentChoiceStates.Valid:
                    return "valid";
                case AssessmentChoiceStates.Default:
                default:
                    return "default";
            }
        }
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Text.HasNothing())
                errors.AddError("Text is required.", nameof(Text));

            if (!IsCorrect.HasValue)
                errors.AddError("Is Correct is required.", nameof(IsCorrect));
        });
    }
}
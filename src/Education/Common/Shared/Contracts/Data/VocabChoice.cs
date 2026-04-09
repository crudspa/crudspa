namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class VocabChoice : Observable, IValidates, IOrderable
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? VocabQuestionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? VocabQuestionWord
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Word
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? IsCorrect
    {
        get;
        set => SetProperty(ref field, value);
    }

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
            if (Word.HasNothing())
                errors.AddError("Word is required.", nameof(Word));
            else if (Word!.Length > 50)
                errors.AddError("Word cannot be longer than 50 characters.", nameof(Word));

            if (!IsCorrect.HasValue)
                errors.AddError("Is Correct is required.", nameof(IsCorrect));
        });
    }
}
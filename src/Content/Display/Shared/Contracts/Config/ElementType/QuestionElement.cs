using Crudspa.Content.Display.Shared.Contracts.Ids;

namespace Crudspa.Content.Display.Shared.Contracts.Config.ElementType;

public class QuestionElement : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ElementId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? QuestionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Question Question
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!ElementId.HasValue)
                errors.AddError("Element is required.", nameof(ElementId));

            errors.AddRange(Question.Validate());
        });
    }
}

public class Question : Observable, IValidates
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AnswerTypeId
    {
        get;
        set
        {
            if (SetProperty(ref field, value))
                EnsureAnswer();
        }
    } = AnswerTypeIds.Text;

    public AnswerType? AnswerType
    {
        get;
        set => SetProperty(ref field, value);
    }

    public BooleanAnswer? BooleanAnswer
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ContactAnswer? ContactAnswer
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateAnswer? DateAnswer
    {
        get;
        set => SetProperty(ref field, value);
    }

    public FileAnswer? FileAnswer
    {
        get;
        set => SetProperty(ref field, value);
    }

    public NumberAnswer? NumberAnswer
    {
        get;
        set => SetProperty(ref field, value);
    }

    public OptionsAnswer? OptionsAnswer
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ScaleAnswer? ScaleAnswer
    {
        get;
        set => SetProperty(ref field, value);
    }

    public TextAnswer? TextAnswer
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Object? Answer => AnswerTypeId switch
    {
        var id when id == AnswerTypeIds.Boolean => BooleanAnswer,
        var id when id == AnswerTypeIds.Contact => ContactAnswer,
        var id when id == AnswerTypeIds.Date => DateAnswer,
        var id when id == AnswerTypeIds.File => FileAnswer,
        var id when id == AnswerTypeIds.Number => NumberAnswer,
        var id when id == AnswerTypeIds.Options => OptionsAnswer,
        var id when id == AnswerTypeIds.Scale => ScaleAnswer,
        var id when id == AnswerTypeIds.Text => TextAnswer,
        _ => null,
    };

    public void EnsureAnswer()
    {
        if (AnswerTypeId == AnswerTypeIds.Boolean)
            BooleanAnswer ??= new();
        else if (AnswerTypeId == AnswerTypeIds.Contact)
            ContactAnswer ??= new();
        else if (AnswerTypeId == AnswerTypeIds.Date)
            DateAnswer ??= new();
        else if (AnswerTypeId == AnswerTypeIds.File)
            FileAnswer ??= new();
        else if (AnswerTypeId == AnswerTypeIds.Number)
            NumberAnswer ??= new();
        else if (AnswerTypeId == AnswerTypeIds.Options)
            OptionsAnswer ??= new();
        else if (AnswerTypeId == AnswerTypeIds.Scale)
            ScaleAnswer ??= new();
        else
        {
            AnswerTypeId = AnswerTypeIds.Text;
            TextAnswer ??= new();
        }

        SetQuestionIds();
        RaisePropertyChanged(nameof(Answer));
    }

    public void SetQuestionIds()
    {
        BooleanAnswer?.SetQuestionId(Id);
        ContactAnswer?.SetQuestionId(Id);
        DateAnswer?.SetQuestionId(Id);
        FileAnswer?.SetQuestionId(Id);
        NumberAnswer?.SetQuestionId(Id);
        OptionsAnswer?.SetQuestionId(Id);
        ScaleAnswer?.SetQuestionId(Id);
        TextAnswer?.SetQuestionId(Id);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (!AnswerTypeId.HasValue)
                errors.AddError("Answer Type is required.", nameof(AnswerTypeId));

            EnsureAnswer();

            if (Answer is IValidates validates)
                errors.AddRange(validates.Validate());
        });
    }
}

public class BooleanAnswer : Observable, IValidates
{
    public enum Kinds { Checkbox, Radio }
    public enum Orientations { Vertical, Horizontal }

    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? QuestionId { get; set => SetProperty(ref field, value); }
    public Kinds Kind { get; set => SetProperty(ref field, value); }
    public Boolean? Default { get; set => SetProperty(ref field, value); }
    public Orientations Orientation { get; set => SetProperty(ref field, value); }
    public String? TrueLabel { get; set => SetProperty(ref field, value); } = "Yes";
    public String? FalseLabel { get; set => SetProperty(ref field, value); } = "No";

    public void SetQuestionId(Guid? questionId) => QuestionId = questionId;

    public List<Error> Validate() => ErrorsEx.Validate(errors =>
    {
        if (TrueLabel.HasSomething() && TrueLabel!.Length > 250)
            errors.AddError("True Label cannot be longer than 250 characters.", nameof(TrueLabel));

        if (Kind == Kinds.Radio && FalseLabel.HasNothing())
            errors.AddError("False Label is required.", nameof(FalseLabel));
        else if (FalseLabel.HasSomething() && FalseLabel!.Length > 250)
            errors.AddError("False Label cannot be longer than 250 characters.", nameof(FalseLabel));
    });
}

public class ContactAnswer : Observable, IValidates
{
    public enum Kinds { Email, Postal, Phone, Signature }

    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? QuestionId { get; set => SetProperty(ref field, value); }
    public Kinds Kind { get; set => SetProperty(ref field, value); }
    public String? Label { get; set => SetProperty(ref field, value); }

    public void SetQuestionId(Guid? questionId) => QuestionId = questionId;

    public void DefaultLabel()
    {
        if (Label.HasSomething())
            return;

        Label = Kind switch
        {
            Kinds.Email => "Email",
            Kinds.Postal => "Address",
            Kinds.Phone => "Phone",
            Kinds.Signature => "Signature",
            _ => null,
        };
    }

    public List<Error> Validate() => ErrorsEx.Validate(errors =>
    {
        if (Label.HasSomething() && Label!.Length > 150)
            errors.AddError("Label cannot be longer than 150 characters.", nameof(Label));
    });
}

public class DateAnswer : Observable
{
    public enum Kinds { Date, Time, DateTime }

    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? QuestionId { get; set => SetProperty(ref field, value); }
    public Kinds Kind { get; set => SetProperty(ref field, value); }
    public DateOnly? DateMin { get; set => SetProperty(ref field, value); }
    public DateOnly? DateMax { get; set => SetProperty(ref field, value); }
    public TimeOnly? TimeMin { get; set => SetProperty(ref field, value); }
    public TimeOnly? TimeMax { get; set => SetProperty(ref field, value); }
    public DateTimeOffset? DateTimeMin { get; set => SetProperty(ref field, value); }
    public DateTimeOffset? DateTimeMax { get; set => SetProperty(ref field, value); }

    public void SetQuestionId(Guid? questionId) => QuestionId = questionId;
}

public class FileAnswer : Observable
{
    public enum Kinds { Audio, Image, Pdf, Video }

    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? QuestionId { get; set => SetProperty(ref field, value); }
    public Kinds Kind { get; set => SetProperty(ref field, value); }

    public void SetQuestionId(Guid? questionId) => QuestionId = questionId;
}

public class NumberAnswer : Observable
{
    public enum Kinds { Integer, Decimal, Currency, Percentage }

    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? QuestionId { get; set => SetProperty(ref field, value); }
    public Kinds Kind { get; set => SetProperty(ref field, value); }
    public Int32? IntegerMin { get; set => SetProperty(ref field, value); }
    public Int32? IntegerMax { get; set => SetProperty(ref field, value); }
    public Single? DecimalMin { get; set => SetProperty(ref field, value); }
    public Single? DecimalMax { get; set => SetProperty(ref field, value); }
    public Single? CurrencyMin { get; set => SetProperty(ref field, value); }
    public Single? CurrencyMax { get; set => SetProperty(ref field, value); }

    public void SetQuestionId(Guid? questionId) => QuestionId = questionId;
}

public class OptionsAnswer : Observable, IValidates
{
    public enum Kinds { Single, Multiple }
    public enum Orientations { Vertical, Horizontal }
    public enum Orderings { Fixed, Random }

    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? QuestionId { get; set => SetProperty(ref field, value); }
    public Kinds Kind { get; set => SetProperty(ref field, value); }
    public Orientations Orientation { get; set => SetProperty(ref field, value); }
    public Boolean? AllowOther { get; set => SetProperty(ref field, value); }
    public String? OtherLabel { get; set => SetProperty(ref field, value); }
    public Int32? MinSelections { get; set => SetProperty(ref field, value); }
    public Int32? MaxSelections { get; set => SetProperty(ref field, value); }
    public Orderings Ordering { get; set => SetProperty(ref field, value); }
    public ObservableCollection<OptionsAnswerChoice> Choices { get; set => SetProperty(ref field, value); } = [];

    public void SetQuestionId(Guid? questionId)
    {
        QuestionId = questionId;
        foreach (var choice in Choices)
            choice.OptionsAnswerId = Id;
    }

    public List<Error> Validate() => ErrorsEx.Validate(errors =>
    {
        if (AllowOther == true && OtherLabel.HasSomething() && OtherLabel!.Length > 50)
            errors.AddError("Other Label cannot be longer than 50 characters.", nameof(OtherLabel));

        if (MinSelections.HasValue && MaxSelections.HasValue && MinSelections > MaxSelections)
            errors.AddError("Minimum selections cannot be greater than maximum selections.", nameof(MinSelections));

        if (Choices.Count == 0)
            errors.AddError("At least one choice is required.", nameof(Choices));

        Choices.Apply(x => errors.AddRange(x.Validate()));
    });
}

public class OptionsAnswerChoice : Observable, IValidates, IOrderable, INamed
{
    public String? Name => Text;

    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? OptionsAnswerId { get; set => SetProperty(ref field, value); }
    public String? Text { get; set => SetProperty(ref field, value); }
    public Int32? Ordinal { get; set => SetProperty(ref field, value); }

    public List<Error> Validate() => ErrorsEx.Validate(errors =>
    {
        if (Text.HasNothing())
            errors.AddError("Choice Text is required.", nameof(Text));
    });
}

public class ScaleAnswer : Observable
{
    public enum Kinds { Rating, Likert }
    public enum RatingKinds { Numbers, Stars }
    public enum LikertKinds { Agreement, Frequency, Importance, Likelihood, Quality }
    public enum Orderings { LowToHigh, HighToLow }

    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? QuestionId { get; set => SetProperty(ref field, value); }
    public Kinds Kind { get; set => SetProperty(ref field, value); }
    public RatingKinds RatingKind { get; set => SetProperty(ref field, value); }
    public LikertKinds LikertKind { get; set => SetProperty(ref field, value); }
    public Int32? RatingMin { get; set => SetProperty(ref field, value); } = 1;
    public Int32? RatingMax { get; set => SetProperty(ref field, value); } = 5;
    public Orderings Ordering { get; set => SetProperty(ref field, value); }

    public void SetQuestionId(Guid? questionId) => QuestionId = questionId;
}

public class TextAnswer : Observable, IValidates
{
    public enum Kinds { Short, Long, Rich }

    public Guid? Id { get; set => SetProperty(ref field, value); }
    public Guid? QuestionId { get; set => SetProperty(ref field, value); }
    public Kinds Kind { get; set => SetProperty(ref field, value); }
    public String? Label { get; set => SetProperty(ref field, value); }
    public String? Placeholder { get; set => SetProperty(ref field, value); }

    public void SetQuestionId(Guid? questionId) => QuestionId = questionId;

    public List<Error> Validate() => ErrorsEx.Validate(errors =>
    {
        if (Label.HasSomething() && Label!.Length > 150)
            errors.AddError("Label cannot be longer than 150 characters.", nameof(Label));

        if (Placeholder.HasSomething() && Placeholder!.Length > 150)
            errors.AddError("Placeholder cannot be longer than 150 characters.", nameof(Placeholder));
    });
}
namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class VocabQuestion : Observable, IValidates, IOrderable, INamed
{
    private String? _word;

    public String? Name => _word;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? VocabPartId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? VocabPartTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Word
    {
        get => _word;
        set => SetProperty(ref _word, value);
    }

    public AudioFile AudioFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Boolean? IsPreview
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? PageBreakBefore
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<VocabChoice> VocabChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<VocabChoiceSelection> Selections
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32 Number { get; set; }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Word.HasNothing())
                errors.AddError("Word is required.", nameof(Word));
            else if (Word!.Length > 50)
                errors.AddError("Word cannot be longer than 50 characters.", nameof(Word));

            if (!IsPreview.HasValue)
                errors.AddError("Is Preview is required.", nameof(IsPreview));

            if (!PageBreakBefore.HasValue)
                errors.AddError("Page Break Before is required.", nameof(PageBreakBefore));

            if (VocabChoices.IsEmpty() || VocabChoices.Count != 4)
                errors.AddError("Exactly 4 choices must be specified.", nameof(VocabChoices));
            else
            {
                if (VocabChoices.Count(x => x.IsCorrect == true) != 2)
                    errors.AddError("Exactly 2 choices must be marked as correct.", nameof(VocabChoices));

                VocabChoices.Apply(x => errors.AddRange(x.Validate()));
            }
        });
    }
}
namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ReadQuestion : Observable, IValidates, IOrderable, INamed
{
    public String Name => $"Question {Ordinal + 1:D}";

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ReadPartId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ReadPartTitle
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
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

    public Boolean? HasCorrectChoice
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Boolean? RequireTextInput
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? CategoryId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? CategoryName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? TypeId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? TypeName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ImageFile ImageFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ReadChoice> ReadChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ReadChoiceSelection> Selections
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Int32 Number { get; set; }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Text.HasNothing())
                errors.AddError("Text is required.", nameof(Text));

            if (!IsPreview.HasValue)
                errors.AddError("Is Preview is required.", nameof(IsPreview));

            if (!PageBreakBefore.HasValue)
                errors.AddError("Page Break Before is required.", nameof(PageBreakBefore));

            if (!HasCorrectChoice.HasValue)
                errors.AddError("Has Correct Choice is required.", nameof(HasCorrectChoice));

            if (!RequireTextInput.HasValue)
                errors.AddError("Require Text Input is required.", nameof(RequireTextInput));

            if (!TypeId.HasValue)
                errors.AddError("Type is required.", nameof(TypeId));

            if (!CategoryId.HasValue)
                errors.AddError("Category is required.", nameof(CategoryId));

            if (RequireTextInput == true)
            {
                if (ReadChoices.HasItems())
                    errors.AddError("Questions that require text input cannot have choices.", nameof(ReadChoices));
            }
            else
            {
                if (ReadChoices.IsEmpty() || ReadChoices.Count < 1)
                    errors.AddError("At least one choice must be added.", nameof(ReadChoices));
                else
                {
                    if (HasCorrectChoice == true && ReadChoices.Count(x => x.IsCorrect == true) != 1)
                        errors.AddError("Exactly one choice must be marked as correct.", nameof(ReadChoices));

                    ReadChoices.Apply(x => errors.AddRange(x.Validate()));
                }
            }
        });
    }
}
namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ListenQuestion : Observable, IValidates, IOrderable, INamed
{
    public String Name => $"Question {Ordinal + 1:D}";

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ListenPartId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ListenPartTitle
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

    public ObservableCollection<ListenChoice> ListenChoices
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ListenChoiceSelection> Selections
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

            if (!CategoryId.HasValue)
                errors.AddError("Category is required.", nameof(CategoryId));

            if (RequireTextInput == true)
            {
                if (ListenChoices.HasItems())
                    errors.AddError("Questions that require text input cannot have choices.", nameof(ListenChoices));
            }
            else
            {
                if (ListenChoices.IsEmpty() || ListenChoices.Count < 1)
                    errors.AddError("At least one choice must be added.", nameof(ListenChoices));
                else
                {
                    if (HasCorrectChoice == true && ListenChoices.Count(x => x.IsCorrect == true) != 1)
                        errors.AddError("Exactly one choice must be marked as correct.", nameof(ListenChoices));

                    ListenChoices.Apply(x => errors.AddRange(x.Validate()));
                }
            }
        });
    }
}
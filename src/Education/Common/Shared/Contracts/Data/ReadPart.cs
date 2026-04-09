namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ReadPart : Observable, IValidates, IOrderable, INamed
{
    public String? Name => Title;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssessmentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? AssessmentName
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? PassageInstructionsText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile PassageInstructionsAudioFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? PreviewInstructionsText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile PreviewInstructionsAudioFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public String? QuestionsInstructionsText
    {
        get;
        set => SetProperty(ref field, value);
    }

    public AudioFile QuestionsInstructionsAudioFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ReadParagraphCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? ReadQuestionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ReadParagraph> ReadParagraphs
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public ObservableCollection<ReadQuestion> ReadQuestions
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Guid? PassageInstructionsAudioFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? PreviewInstructionsAudioFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? QuestionsInstructionsAudioFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (Title.HasNothing())
                errors.AddError("Title is required.", nameof(Title));
            else if (Title!.Length > 100)
                errors.AddError("Title cannot be longer than 100 characters.", nameof(Title));
        });
    }
}
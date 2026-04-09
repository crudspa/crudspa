namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ListenPart : Observable, IValidates, IOrderable, INamed
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

    public AudioFile PassageAudioFileFile
    {
        get;
        set => SetProperty(ref field, value);
    } = new();

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

    public Int32? ListenQuestionCount
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Int32? Ordinal
    {
        get;
        set => SetProperty(ref field, value);
    }

    public ObservableCollection<ListenQuestion> ListenQuestions
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Guid? PassageAudioFileId
    {
        get;
        set => SetProperty(ref field, value);
    }

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

            if (PassageAudioFileFile.Name.HasNothing())
                errors.AddError("Passage Audio is required.", nameof(PassageAudioFileFile));
        });
    }
}
namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class VocabChoiceSelection : Observable, IUnique
{
    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? AssignmentId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public List<Guid?> Choices
    {
        get;
        set => SetProperty(ref field, value);
    } = [];

    public Guid? ChoiceId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public DateTimeOffset? Made
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? ChoiceVocabQuestionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? ChoiceText
    {
        get;
        set => SetProperty(ref field, value);
    }
}
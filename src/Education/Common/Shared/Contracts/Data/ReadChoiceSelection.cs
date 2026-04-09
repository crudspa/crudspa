namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ReadChoiceSelection : Observable, IUnique
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

    public Guid? ChoiceReadQuestionId
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
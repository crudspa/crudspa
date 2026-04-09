namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ListenChoiceSelection : Observable, IUnique
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

    public Guid? ChoiceListenQuestionId
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
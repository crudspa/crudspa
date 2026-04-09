namespace Crudspa.Education.Common.Shared.Contracts.Data;

public class ListenTextEntry : Observable, IUnique
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

    public Guid? QuestionId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Text
    {
        get;
        set => SetProperty(ref field, value);
    }
}
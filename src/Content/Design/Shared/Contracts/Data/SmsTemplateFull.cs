namespace Crudspa.Content.Design.Shared.Contracts.Data;

public class SmsTemplateFull : Observable, INamed
{
    public String? Name => Title;

    public Guid? Id
    {
        get;
        set => SetProperty(ref field, value);
    }

    public Guid? MembershipId
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Title
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? Body
    {
        get;
        set => SetProperty(ref field, value);
    }

    public String? MembershipName
    {
        get;
        set => SetProperty(ref field, value);
    }
}
namespace Crudspa.Content.Jobs.Shared.Contracts.Config;

public class SendSmsConfig : Observable, IValidates
{
    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors => { });
    }
}
namespace Crudspa.Content.Jobs.Shared.Contracts.Config;

public class SendEmailsConfig : Observable, IValidates
{
    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors => { });
    }
}
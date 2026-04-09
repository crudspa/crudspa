namespace Crudspa.Framework.Jobs.Shared.Contracts.Config.JobType;

public class ExpireSessionsConfig : Observable
{
    public Int32? SessionLengthInDays
    {
        get;
        set => SetProperty(ref field, value);
    } = 90;

    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors =>
        {
            if (SessionLengthInDays is null || SessionLengthInDays < 1)
                errors.AddError("Session Length In Days must be greater than 0.", nameof(SessionLengthInDays));
        });
    }
}
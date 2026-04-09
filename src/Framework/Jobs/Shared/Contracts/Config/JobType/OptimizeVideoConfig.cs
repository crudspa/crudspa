namespace Crudspa.Framework.Jobs.Shared.Contracts.Config.JobType;

public class OptimizeVideoConfig : Observable
{
    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors => { });
    }
}
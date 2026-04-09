namespace Crudspa.Framework.Jobs.Shared.Contracts.Config.JobType;

public class OptimizeAudioConfig : Observable
{
    public List<Error> Validate()
    {
        return ErrorsEx.Validate(errors => { });
    }
}
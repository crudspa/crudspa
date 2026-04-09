namespace Crudspa.Framework.Jobs.Shared.Contracts.Behavior;

public interface IJobsConfigService
{
    JobsConfig Fetch();
    void Invalidate();
}
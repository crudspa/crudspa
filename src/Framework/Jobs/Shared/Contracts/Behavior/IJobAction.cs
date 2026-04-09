namespace Crudspa.Framework.Jobs.Shared.Contracts.Behavior;

public interface IJobAction
{
    void Configure(Guid? sessionId, String json);
    Task<Boolean> Run(Guid? jobId);
}
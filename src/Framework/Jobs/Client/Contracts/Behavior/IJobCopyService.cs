namespace Crudspa.Framework.Jobs.Client.Contracts.Behavior;

public interface IJobCopyService
{
    void Push(Job job);
    Job? Pop();
}
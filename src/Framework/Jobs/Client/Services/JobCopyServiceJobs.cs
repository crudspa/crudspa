namespace Crudspa.Framework.Jobs.Client.Services;

public class JobCopyServiceJobs : IJobCopyService
{
    private Job? _copy;

    public void Push(Job job)
    {
        job.Id = Guid.NewGuid();
        job.Added = null;
        job.Started = null;
        job.Ended = null;

        _copy = job.DeepClone();
    }

    public Job? Pop()
    {
        var result = _copy;
        _copy = null;
        return result;
    }
}
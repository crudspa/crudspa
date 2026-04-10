namespace Crudspa.Framework.Jobs.Client.Services;

public static class JobStatusChangeEx
{
    extension(Job job)
    {
        public void ApplyStatusChange(JobStatusChanged payload, IList<OrderableCssClass> jobStatusNames, IList<Named> deviceNames)
        {
            job.Started = payload.Started;
            job.Ended = payload.Ended;
            job.StatusId = payload.StatusId;
            job.DeviceId = payload.DeviceId;

            var status = jobStatusNames.FirstOrDefault(x => x.Id.Equals(payload.StatusId));
            job.StatusName = status?.Name;
            job.StatusCssClass = status?.CssClass;

            job.DeviceName = deviceNames.FirstOrDefault(x => x.Id.Equals(payload.DeviceId))?.Name;
        }
    }
}
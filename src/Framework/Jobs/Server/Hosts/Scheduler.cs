using Crudspa.Framework.Jobs.Shared.Extensions;

namespace Crudspa.Framework.Jobs.Server.Hosts;

public class Scheduler : BackgroundService
{
    private readonly IGatewayService _gatewayService;
    private readonly ILogger<Scheduler> _logger;
    private readonly IServerConfigService _serverConfigService;
    private readonly IJobsConfigService _jobsConfigService;
    private readonly IJobRunService _jobRunService;
    private readonly IJobScheduleService _jobScheduleService;
    private readonly JobsConfig _jobsConfig;
    private Session _session = null!;

    public Scheduler(
        ILogger<Scheduler> logger,
        IGatewayService gatewayService,
        IServerConfigService serverConfigService,
        IJobsConfigService jobsConfigService,
        IJobRunService jobRunService,
        IJobScheduleService jobScheduleService)
    {
        _gatewayService = gatewayService;
        _logger = logger;
        _serverConfigService = serverConfigService;
        _jobsConfigService = jobsConfigService;
        _jobRunService = jobRunService;
        _jobScheduleService = jobScheduleService;

        _jobsConfig = _jobsConfigService.Fetch();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        _logger.LogInformation("Scheduler service {className} started.", GetType().FullName);

        var serverConfig = _serverConfigService.Fetch();
        var jobsConfig = _jobsConfigService.Fetch();

        if (jobsConfig.SchedulingInterval <= 0)
            _logger.LogWarning("SchedulingInterval is set to {interval} so the scheduler will not run on this device.", jobsConfig.SchedulingInterval);
        else
        {
            _logger.LogInformation("Starting scheduler session for portal {portalId} and user {userId}...", serverConfig.PortalId, jobsConfig.UserId);

            var sessionResponse = await _jobRunService.CreateSession(new Request());

            if (!sessionResponse.Ok)
            {
                _logger.LogCritical("Could not start scheduler session. {message}", sessionResponse.ErrorMessages);
                return;
            }

            _session = sessionResponse.Value;

            _logger.LogInformation("Started scheduler session {sessionId}.", _session.Id);

            _logger.LogInformation("Updating 'Next Run' for all overdue schedules on device {deviceId}...", _jobsConfig.DeviceId);

            var response = await _jobScheduleService.RescheduleOverdue(new(_session.Id, new() { Id = _jobsConfig.DeviceId }));

            if (!response.Ok)
                _logger.LogError("Error rescheduling overdue schedules for device {deviceId}. {message}", _jobsConfig.DeviceId, response.ErrorMessages);

            _logger.LogInformation("Beginning scheduler loop for device {deviceId} with polling interval {interval}...", _jobsConfig.DeviceId, _jobsConfig.SchedulingInterval);

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await ProcessBatch();
                    await Task.Delay(_jobsConfig.SchedulingInterval, stoppingToken);
                }
                catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Exception raised during scheduler loop.");
                }
            }

            _logger.LogInformation("Exited batch scheduler loop.");
        }
    }

    private async Task ProcessBatch()
    {
        _logger.LogTrace("Checking for job schedules...");

        var fetchBatchResponse = await _jobScheduleService.FetchBatch(new(_session.Id, new() { Id = _jobsConfig.DeviceId }));

        if (!fetchBatchResponse.Ok)
        {
            _logger.LogError("Error fetching job schedules. {error}", fetchBatchResponse.ErrorMessages);
            return;
        }

        var jobSchedules = fetchBatchResponse.Value;

        if (jobSchedules.IsEmpty())
            return;

        _logger.LogInformation("Found {jobCount} job schedules.", jobSchedules.Count);

        foreach (var schedule in jobSchedules)
        {
            var updatedSchedule = new JobSchedule
            {
                Id = schedule.Id,
                NextRun = schedule.DetermineNextRunDate(),
            };

            var createJobResponse = await _jobScheduleService.CreateJob(new(_session.Id, updatedSchedule));

            if (!createJobResponse.Ok)
                _logger.LogError("Error creating job. {error}", createJobResponse.ErrorMessages);
            else
            {
                _logger.LogInformation("Created job for schedule {scheduleId}. The next scheduled run is: {nextRunDate}", updatedSchedule.Id, updatedSchedule.NextRun);

                if (updatedSchedule.Id is not null)
                    await _gatewayService.Publish(new JobScheduleSaved { Id = updatedSchedule.Id });

                if (createJobResponse.Value?.Id is not null)
                    await _gatewayService.Publish(new JobAdded { Id = createJobResponse.Value.Id });
            }
        }
    }
}
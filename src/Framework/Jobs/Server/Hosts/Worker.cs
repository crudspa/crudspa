using Crudspa.Framework.Jobs.Shared.Contracts.Ids;

namespace Crudspa.Framework.Jobs.Server.Hosts;

public class Worker : BackgroundService
{
    private readonly ILogger<Worker> _logger;
    private readonly IGatewayService _gatewayService;
    private readonly IServiceProvider _serviceProvider;
    private readonly IServerConfigService _serverConfigService;
    private readonly IJobsConfigService _jobsConfigService;
    private readonly IJobRunService _jobRunService;
    private readonly JobsConfig _jobsConfig;
    private Session _session = null!;

    public Worker(
        ILogger<Worker> logger,
        IGatewayService gatewayService,
        IServiceProvider serviceProvider,
        IServerConfigService serverConfigService,
        IJobsConfigService jobsConfigService,
        IJobRunService jobRunService)
    {
        _logger = logger;
        _gatewayService = gatewayService;
        _serviceProvider = serviceProvider;
        _serverConfigService = serverConfigService;
        _jobsConfigService = jobsConfigService;
        _jobRunService = jobRunService;

        _jobsConfig = _jobsConfigService.Fetch();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        await Task.Yield();

        _logger.LogInformation("Worker service {className} started.", GetType().FullName);

        var serverConfig = _serverConfigService.Fetch();
        var jobsConfig = _jobsConfigService.Fetch();

        _logger.LogInformation("Starting worker session for portal {portalId} and user {userId}...", serverConfig.PortalId, jobsConfig.UserId);

        var sessionResponse = await _jobRunService.CreateSession(new Request());

        if (!sessionResponse.Ok)
        {
            _logger.LogCritical("Could not start worker session. {message}", sessionResponse.ErrorMessages);
            return;
        }

        _session = sessionResponse.Value;

        _logger.LogInformation("Started worker session {sessionId}.", _session.Id);

        _logger.LogInformation("Marking all 'Running' jobs for device {deviceId} as 'Canceled'...", _jobsConfig.DeviceId);

        var response = await _jobRunService.CancelRunning(new(_session.Id, new() { Id = _jobsConfig.DeviceId }));

        if (!response.Ok)
            _logger.LogError("Error while marking jobs as canceled for device {deviceId}. {message}", _jobsConfig.DeviceId, response.ErrorMessages);
        else
            await PublishStatusChanges(response.Value);

        _logger.LogInformation("Beginning worker loop for device {deviceId} with polling interval {interval}...", _jobsConfig.DeviceId, _jobsConfig.PollingInterval);

        await ProcessBatch();

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await Task.Delay(_jobsConfig.PollingInterval, stoppingToken);
                await ProcessBatch();
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception raised during worker loop.");
            }
        }

        _logger.LogInformation("Exited worker loop.");
    }

    private async Task ProcessBatch()
    {
        _logger.LogTrace("Checking for jobs...");

        var request = new Request<Device>(_session.Id, new() { Id = _jobsConfig.DeviceId });
        var response = await _jobRunService.FetchBatch(request);

        if (!response.Ok)
        {
            _logger.LogError("Error fetching batch. {error}", response.ErrorMessages);
            return;
        }

        var jobs = response.Value;
        if (jobs.IsEmpty())
            return;

        _logger.LogInformation("Found {jobCount} jobs.", jobs.Count);

        await PublishStatusChanges(jobs.Select(ToJobStatusChanged));

        var batchId = jobs.First().BatchId;
        using (_logger.BeginScope(new Dictionary<String, Object> { ["BatchId"] = batchId! }))
        {
            _logger.LogDebug("Starting batch {batchId}...", batchId);

            var running = jobs
                .Select(RunJob)
                .ToList();

            try
            {
                await Task.WhenAll(running);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "One or more jobs failed in batch {batchId}.", batchId);
            }

            _logger.LogDebug("Completed batch {batchId}.", batchId);
        }
    }

    private async Task RunJob(Job job)
    {
        using (_logger.BeginScope(new Dictionary<String, Object> { ["JobId"] = job.Id! }))
        {
            _logger.LogInformation("Starting job {jobId}: {description}", job.Id, job.Description);

            try
            {
                _logger.LogDebug("Instantiating type {jobTypeActionClass}...", job.Type!.ActionClass);

                var type = Type.GetType(job.Type.ActionClass!);

                if (type is null)
                    throw new($"Type {job.Type!.ActionClass} could not be found.");

                var action = (IJobAction)ActivatorUtilities.CreateInstance(_serviceProvider, type);

                _logger.LogDebug("Configuring {typeName}...", type.FullName);

                action.Configure(_session.Id, job.Config!);

                _logger.LogDebug("Running {typeName}...", type.FullName);

                var success = await action.Run(job.Id);

                job.Ended = DateTimeOffset.Now;
                job.StatusId = success ? JobStatusIds.Completed : JobStatusIds.Failed;

                _logger.LogInformation("Job {jobId} ended.", job.Id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception raised while running job {jobId}.", job.Id);

                job.Ended = DateTimeOffset.Now;
                job.StatusId = JobStatusIds.Failed;
            }

            await UpdateStatus(job);
        }
    }

    private async Task UpdateStatus(Job job)
    {
        var response = await _jobRunService.SaveStatus(new(_session.Id, job));

        if (!response.Ok)
        {
            _logger.LogError("Call to IJobRunService.SaveStatus() failed for job {jobId}. {error}", job.Id, response.ErrorMessages);
            return;
        }

        if (job.Id is not null)
            await PublishStatusChange(ToJobStatusChanged(job));
    }

    private async Task PublishStatusChanges(IEnumerable<JobStatusChanged>? changes)
    {
        if (changes is null)
            return;

        foreach (var change in changes)
            await PublishStatusChange(change);
    }

    private async Task PublishStatusChange(JobStatusChanged change)
    {
        if (change.Id is null)
            return;

        await _gatewayService.Publish(change);
    }

    private static JobStatusChanged ToJobStatusChanged(Job job)
    {
        return new()
        {
            Id = job.Id,
            Started = job.Started,
            Ended = job.Ended,
            StatusId = job.StatusId,
            DeviceId = job.DeviceId,
        };
    }
}
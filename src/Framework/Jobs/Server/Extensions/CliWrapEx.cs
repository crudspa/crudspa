using CliWrap;
using CliWrap.EventStream;

namespace Crudspa.Framework.Jobs.Server.Extensions;

public static class CliWrapEx
{
    extension(Command command)
    {
        public async Task<Boolean> RunAndLog<T>(ILogger<T> logger, Boolean failOnStandardErrorEvent = false, CancellationTokenSource? cancellationTokenSource = null)
        {
            logger.LogInformation("Running command: {command} {arguments}", command.TargetFilePath, command.Arguments);

            var standardErrorEventRaised = false;
            var exceptionRaised = false;
            var exitCode = 0;

            var ownsCts = cancellationTokenSource is null;
            var tokenSource = cancellationTokenSource ?? new CancellationTokenSource();

            try
            {
                await foreach (var commandEvent in command.ListenAsync(tokenSource.Token))
                    switch (commandEvent)
                    {
                        case StartedCommandEvent startedEvent:
                            logger.LogInformation("Process {processId} started.", startedEvent.ProcessId);
                            break;
                        case ExitedCommandEvent exitedEvent:
                            logger.LogInformation("Process exited with code {exitCode}.", exitedEvent.ExitCode);
                            exitCode = exitedEvent.ExitCode;
                            break;
                        case StandardOutputCommandEvent outputEvent:
                            logger.LogInformation("Output: {output}", outputEvent.Text);
                            break;
                        case StandardErrorCommandEvent errorEvent:
                            logger.LogError("ERROR: {errorText}", errorEvent.Text);
                            standardErrorEventRaised = true;
                            break;
                    }
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "EXCEPTION: {message}", ex.Message);
                exceptionRaised = true;
            }
            finally
            {
                if (ownsCts)
                    tokenSource.Dispose();
            }

            return exitCode == 0
                && !exceptionRaised
                && (!failOnStandardErrorEvent || !standardErrorEventRaised);
        }
    }
}
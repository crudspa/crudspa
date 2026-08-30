using Crudspa.Education.Rostering.Server.Sproxies;
using Crudspa.Education.Rostering.Shared.Contracts.Behavior;
using Crudspa.Education.Rostering.Shared.Contracts.Data;
using Crudspa.Framework.Core.Server.Contracts.Behavior;

namespace Crudspa.Education.Rostering.Server.Services;

public class RosterStageSinkSql(ISqlWrappers sqlWrappers, Guid runId) : IRosterSink
{
    public String? Checkpoint { get; private set; }

    public async Task Write(RosterBatch batch, CancellationToken cancellationToken = default)
    {
        cancellationToken.ThrowIfCancellationRequested();

        if (batch.Schools.Count > 0
            || batch.Terms.Count > 0
            || batch.Courses.Count > 0
            || batch.Classes.Count > 0
            || batch.People.Count > 0
            || batch.Roles.Count > 0
            || batch.Enrollments.Count > 0)
            await sqlWrappers.WithTransaction(async (connection, transaction) =>
                await RosterBatchInsert.Execute(connection, transaction, runId, batch));

        Checkpoint = batch.Checkpoint ?? Checkpoint;
    }
}
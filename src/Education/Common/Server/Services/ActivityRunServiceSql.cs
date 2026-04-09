namespace Crudspa.Education.Common.Server.Services;

public class ActivityRunServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers) : IActivityRunService
{
    public async Task<Response<ActivityAssignment>> AddActivityState(Request<ActivityAssignment> request)
    {
        return await wrappers.Try<ActivityAssignment>(request, async response =>
        {
            var activityAssignment = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                activityAssignment.Id = await ActivityAssignmentInsert.Execute(connection, transaction, request.SessionId, activityAssignment);

                foreach (var activityChoiceSelection in activityAssignment.ActivityChoiceSelections)
                {
                    activityChoiceSelection.AssignmentId = activityAssignment.Id;
                    await ActivityChoiceSelectionInsert.Execute(connection, transaction, request.SessionId, activityChoiceSelection);
                }

                foreach (var textEntry in activityAssignment.TextEntries.Where(x => x.Text.HasSomething()))
                {
                    textEntry.AssignmentId = activityAssignment.Id;
                    await ActivityTextEntryInsert.Execute(connection, transaction, request.SessionId, textEntry);
                }
            });

            return activityAssignment;
        });
    }

    public async Task<Response> SaveActivityState(Request<ActivityAssignment> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var activityAssignment = request.Value;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ActivityAssignmentUpdate.Execute(connection, transaction, request.SessionId, activityAssignment);

                foreach (var activityChoiceSelection in activityAssignment.ActivityChoiceSelections)
                    await ActivityChoiceSelectionInsert.Execute(connection, transaction, request.SessionId, activityChoiceSelection);

                foreach (var textEntry in activityAssignment.TextEntries.Where(x => x.Text.HasSomething()))
                    await ActivityTextEntryInsert.Execute(connection, transaction, request.SessionId, textEntry);
            });
        });
    }
}
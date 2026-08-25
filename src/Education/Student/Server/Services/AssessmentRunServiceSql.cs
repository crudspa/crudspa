namespace Crudspa.Education.Student.Server.Services;

public class AssessmentRunServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IAssessmentProgressService assessmentProgressService)
    : IAssessmentRunService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<AssessmentAssignment>>> FetchAssessments(Request request)
    {
        return await wrappers.Try<IList<AssessmentAssignment>>(request, async response =>
        {
            return await AssessmentSelectForStudent.Execute(Connection, request.SessionId);
        });
    }

    public async Task<Response<Assessment?>> FetchAssessment(Request<AssessmentAssignment> request)
    {
        if (!await IsAccessible(request.SessionId, request.Value.Id))
            return new("Assessment assignment was not found or is not licensed for this student.");

        await sqlWrappers.WithConnection(async (connection, transaction) =>
        {
            await AssessmentAssignmentMarkStarted.Execute(connection, transaction, request.SessionId, request.Value.Id);
        });

        var response = await assessmentProgressService.Fetch(request);

        if (response.Ok)
        {
            // Don't send "IsCorrect" info to the student app, except for the preview questions

            foreach (var part in response.Value.VocabParts)
            foreach (var question in part.VocabQuestions.Where(x => x.IsPreview != true))
            foreach (var choice in question.VocabChoices)
                choice.IsCorrect = null;

            foreach (var part in response.Value.ListenParts)
            foreach (var question in part.ListenQuestions.Where(x => x.IsPreview != true))
            foreach (var choice in question.ListenChoices)
                choice.IsCorrect = null;

            foreach (var part in response.Value.ReadParts)
            foreach (var question in part.ReadQuestions.Where(x => x.IsPreview != true))
            foreach (var choice in question.ReadChoices)
                choice.IsCorrect = null;
        }

        return response;
    }

    public async Task<Response> AddVocabChoiceSelection(Request<VocabChoiceSelection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var selection = request.Value;

            if (!await RequireAccessible(response, request.SessionId, selection.AssignmentId)) return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await VocabChoiceSelectionInsert.Execute(connection, transaction, request.SessionId, selection);
            });
        });
    }

    public async Task<Response> AddListenChoiceSelection(Request<ListenChoiceSelection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var selection = request.Value;

            if (!await RequireAccessible(response, request.SessionId, selection.AssignmentId)) return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ListenChoiceSelectionInsert.Execute(connection, transaction, request.SessionId, selection);
            });
        });
    }

    public async Task<Response> AddReadChoiceSelection(Request<ReadChoiceSelection> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var selection = request.Value;

            if (!await RequireAccessible(response, request.SessionId, selection.AssignmentId)) return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ReadChoiceSelectionInsert.Execute(connection, transaction, request.SessionId, selection);
            });
        });
    }

    public async Task<Response> AddVocabPartCompleted(Request<VocabPartCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var vocabPartCompleted = request.Value;

            if (!await RequireAccessible(response, request.SessionId, vocabPartCompleted.AssignmentId)) return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await VocabPartCompletedInsert.Execute(connection, transaction, request.SessionId, vocabPartCompleted);
            });
        });
    }

    public async Task<Response> AddListenPartCompleted(Request<ListenPartCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var listenPartCompleted = request.Value;

            if (!await RequireAccessible(response, request.SessionId, listenPartCompleted.AssignmentId)) return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ListenPartCompletedInsert.Execute(connection, transaction, request.SessionId, listenPartCompleted);

                foreach (var textEntry in listenPartCompleted.TextEntries)
                    await ListenTextEntryInsert.Execute(connection, transaction, request.SessionId, textEntry);
            });
        });
    }

    public async Task<Response> AddReadPartCompleted(Request<ReadPartCompleted> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var readPartCompleted = request.Value;

            if (!await RequireAccessible(response, request.SessionId, readPartCompleted.AssignmentId)) return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ReadPartCompletedInsert.Execute(connection, transaction, request.SessionId, readPartCompleted);

                foreach (var textEntry in readPartCompleted.TextEntries)
                    await ReadTextEntryInsert.Execute(connection, transaction, request.SessionId, textEntry);
            });
        });
    }

    public async Task<Response> AddAssessmentCompleted(Request<AssessmentAssignment> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var assessmentAssignment = request.Value;

            if (!await RequireAccessible(response, request.SessionId, assessmentAssignment.Id)) return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await AssessmentAssignmentMarkCompleted.Execute(connection, transaction, request.SessionId, assessmentAssignment.Id);
            });
        });
    }

    private async Task<Boolean> IsAccessible(Guid? sessionId, Guid? assignmentId) =>
        assignmentId.HasValue && await AssessmentAssignmentIsAccessible.Execute(Connection, sessionId, assignmentId);

    private async Task<Boolean> RequireAccessible(Response response, Guid? sessionId, Guid? assignmentId)
    {
        if (await IsAccessible(sessionId, assignmentId)) return true;

        response.AddError("Assessment assignment was not found or is not licensed for this student.");
        return false;
    }
}
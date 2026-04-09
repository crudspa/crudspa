namespace Crudspa.Education.Publisher.Server.Services;

public class ListenPartServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IListenPartService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ListenPart>>> FetchForAssessment(Request<Assessment> request)
    {
        return await wrappers.Try<IList<ListenPart>>(request, async response =>
        {
            var listenParts = await ListenPartSelectForAssessment.Execute(Connection, request.SessionId, request.Value.Id);
            return listenParts;
        });
    }

    public async Task<Response<ListenPart?>> Fetch(Request<ListenPart> request)
    {
        return await wrappers.Try<ListenPart?>(request, async response =>
        {
            var listenPart = await ListenPartSelect.Execute(Connection, request.SessionId, request.Value);
            return listenPart;
        });
    }

    public async Task<Response<ListenPart?>> Add(Request<ListenPart> request)
    {
        return await wrappers.Validate<ListenPart?, ListenPart>(request, async response =>
        {
            var listenPart = request.Value;

            var passageAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenPart.PassageAudioFileFile));
            if (!passageAudioFileFileResponse.Ok)
            {
                response.AddErrors(passageAudioFileFileResponse.Errors);
                return null;
            }

            listenPart.PassageAudioFileFile.Id = passageAudioFileFileResponse.Value.Id;

            var passageInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenPart.PassageInstructionsAudioFileFile));
            if (!passageInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(passageInstructionsAudioFileFileResponse.Errors);
                return null;
            }

            listenPart.PassageInstructionsAudioFileFile.Id = passageInstructionsAudioFileFileResponse.Value.Id;

            var previewInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenPart.PreviewInstructionsAudioFileFile));
            if (!previewInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(previewInstructionsAudioFileFileResponse.Errors);
                return null;
            }

            listenPart.PreviewInstructionsAudioFileFile.Id = previewInstructionsAudioFileFileResponse.Value.Id;

            var questionsInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenPart.QuestionsInstructionsAudioFileFile));
            if (!questionsInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(questionsInstructionsAudioFileFileResponse.Errors);
                return null;
            }

            listenPart.QuestionsInstructionsAudioFileFile.Id = questionsInstructionsAudioFileFileResponse.Value.Id;

            listenPart.PassageInstructionsText = htmlSanitizer.Sanitize(listenPart.PassageInstructionsText);
            listenPart.PreviewInstructionsText = htmlSanitizer.Sanitize(listenPart.PreviewInstructionsText);
            listenPart.QuestionsInstructionsText = htmlSanitizer.Sanitize(listenPart.QuestionsInstructionsText);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ListenPartInsert.Execute(connection, transaction, request.SessionId, listenPart);

                return new ListenPart
                {
                    Id = id,
                    AssessmentId = listenPart.AssessmentId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ListenPart> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var listenPart = request.Value;

            var existing = await ListenPartSelect.Execute(Connection, request.SessionId, listenPart);

            var passageAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenPart.PassageAudioFileFile), existing?.PassageAudioFileFile);
            if (!passageAudioFileFileResponse.Ok)
            {
                response.AddErrors(passageAudioFileFileResponse.Errors);
                return;
            }

            listenPart.PassageAudioFileFile.Id = passageAudioFileFileResponse.Value.Id;

            var passageInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenPart.PassageInstructionsAudioFileFile), existing?.PassageInstructionsAudioFileFile);
            if (!passageInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(passageInstructionsAudioFileFileResponse.Errors);
                return;
            }

            listenPart.PassageInstructionsAudioFileFile.Id = passageInstructionsAudioFileFileResponse.Value.Id;

            var previewInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenPart.PreviewInstructionsAudioFileFile), existing?.PreviewInstructionsAudioFileFile);
            if (!previewInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(previewInstructionsAudioFileFileResponse.Errors);
                return;
            }

            listenPart.PreviewInstructionsAudioFileFile.Id = previewInstructionsAudioFileFileResponse.Value.Id;

            var questionsInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenPart.QuestionsInstructionsAudioFileFile), existing?.QuestionsInstructionsAudioFileFile);
            if (!questionsInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(questionsInstructionsAudioFileFileResponse.Errors);
                return;
            }

            listenPart.QuestionsInstructionsAudioFileFile.Id = questionsInstructionsAudioFileFileResponse.Value.Id;

            listenPart.PassageInstructionsText = htmlSanitizer.Sanitize(listenPart.PassageInstructionsText);
            listenPart.PreviewInstructionsText = htmlSanitizer.Sanitize(listenPart.PreviewInstructionsText);
            listenPart.QuestionsInstructionsText = htmlSanitizer.Sanitize(listenPart.QuestionsInstructionsText);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ListenPartUpdate.Execute(connection, transaction, request.SessionId, listenPart);
            });
        });
    }

    public async Task<Response> Remove(Request<ListenPart> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var listenPart = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ListenPartDelete.Execute(connection, transaction, request.SessionId, listenPart);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<ListenPart>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var listenParts = request.Value;

            listenParts.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ListenPartUpdateOrdinals.Execute(connection, transaction, request.SessionId, listenParts);
            });
        });
    }
}
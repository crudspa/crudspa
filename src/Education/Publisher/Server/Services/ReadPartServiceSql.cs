namespace Crudspa.Education.Publisher.Server.Services;

public class ReadPartServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IReadPartService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ReadPart>>> FetchForAssessment(Request<Assessment> request)
    {
        return await wrappers.Try<IList<ReadPart>>(request, async response =>
        {
            var readParts = await ReadPartSelectForAssessment.Execute(Connection, request.SessionId, request.Value.Id);
            return readParts;
        });
    }

    public async Task<Response<ReadPart?>> Fetch(Request<ReadPart> request)
    {
        return await wrappers.Try<ReadPart?>(request, async response =>
        {
            var readPart = await ReadPartSelect.Execute(Connection, request.SessionId, request.Value);
            return readPart;
        });
    }

    public async Task<Response<ReadPart?>> Add(Request<ReadPart> request)
    {
        return await wrappers.Validate<ReadPart?, ReadPart>(request, async response =>
        {
            var readPart = request.Value;

            var passageInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readPart.PassageInstructionsAudioFileFile));
            if (!passageInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(passageInstructionsAudioFileFileResponse.Errors);
                return null;
            }

            readPart.PassageInstructionsAudioFileFile.Id = passageInstructionsAudioFileFileResponse.Value.Id;

            var previewInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readPart.PreviewInstructionsAudioFileFile));
            if (!previewInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(previewInstructionsAudioFileFileResponse.Errors);
                return null;
            }

            readPart.PreviewInstructionsAudioFileFile.Id = previewInstructionsAudioFileFileResponse.Value.Id;

            var questionsInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readPart.QuestionsInstructionsAudioFileFile));
            if (!questionsInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(questionsInstructionsAudioFileFileResponse.Errors);
                return null;
            }

            readPart.QuestionsInstructionsAudioFileFile.Id = questionsInstructionsAudioFileFileResponse.Value.Id;

            readPart.PassageInstructionsText = htmlSanitizer.Sanitize(readPart.PassageInstructionsText);
            readPart.PreviewInstructionsText = htmlSanitizer.Sanitize(readPart.PreviewInstructionsText);
            readPart.QuestionsInstructionsText = htmlSanitizer.Sanitize(readPart.QuestionsInstructionsText);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await ReadPartInsert.Execute(connection, transaction, request.SessionId, readPart);

                return new ReadPart
                {
                    Id = id,
                    AssessmentId = readPart.AssessmentId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ReadPart> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var readPart = request.Value;

            var existing = await ReadPartSelect.Execute(Connection, request.SessionId, readPart);

            var passageInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readPart.PassageInstructionsAudioFileFile), existing?.PassageInstructionsAudioFileFile);
            if (!passageInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(passageInstructionsAudioFileFileResponse.Errors);
                return;
            }

            readPart.PassageInstructionsAudioFileFile.Id = passageInstructionsAudioFileFileResponse.Value.Id;

            var previewInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readPart.PreviewInstructionsAudioFileFile), existing?.PreviewInstructionsAudioFileFile);
            if (!previewInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(previewInstructionsAudioFileFileResponse.Errors);
                return;
            }

            readPart.PreviewInstructionsAudioFileFile.Id = previewInstructionsAudioFileFileResponse.Value.Id;

            var questionsInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readPart.QuestionsInstructionsAudioFileFile), existing?.QuestionsInstructionsAudioFileFile);
            if (!questionsInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(questionsInstructionsAudioFileFileResponse.Errors);
                return;
            }

            readPart.QuestionsInstructionsAudioFileFile.Id = questionsInstructionsAudioFileFileResponse.Value.Id;

            readPart.PassageInstructionsText = htmlSanitizer.Sanitize(readPart.PassageInstructionsText);
            readPart.PreviewInstructionsText = htmlSanitizer.Sanitize(readPart.PreviewInstructionsText);
            readPart.QuestionsInstructionsText = htmlSanitizer.Sanitize(readPart.QuestionsInstructionsText);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ReadPartUpdate.Execute(connection, transaction, request.SessionId, readPart);
            });
        });
    }

    public async Task<Response> Remove(Request<ReadPart> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var readPart = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ReadPartDelete.Execute(connection, transaction, request.SessionId, readPart);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<ReadPart>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var readParts = request.Value;

            readParts.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ReadPartUpdateOrdinals.Execute(connection, transaction, request.SessionId, readParts);
            });
        });
    }
}
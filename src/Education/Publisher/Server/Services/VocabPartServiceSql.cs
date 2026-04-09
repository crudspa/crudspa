namespace Crudspa.Education.Publisher.Server.Services;

public class VocabPartServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IVocabPartService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<VocabPart>>> FetchForAssessment(Request<Assessment> request)
    {
        return await wrappers.Try<IList<VocabPart>>(request, async response =>
        {
            var vocabParts = await VocabPartSelectForAssessment.Execute(Connection, request.SessionId, request.Value.Id);

            return vocabParts;
        });
    }

    public async Task<Response<VocabPart?>> Fetch(Request<VocabPart> request)
    {
        return await wrappers.Try<VocabPart?>(request, async response =>
        {
            var vocabPart = await VocabPartSelect.Execute(Connection, request.SessionId, request.Value);

            return vocabPart;
        });
    }

    public async Task<Response<VocabPart?>> Add(Request<VocabPart> request)
    {
        return await wrappers.Validate<VocabPart?, VocabPart>(request, async response =>
        {
            var vocabPart = request.Value;

            var previewInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, vocabPart.PreviewInstructionsAudioFileFile));
            if (!previewInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(previewInstructionsAudioFileFileResponse.Errors);
                return null;
            }

            vocabPart.PreviewInstructionsAudioFileFile.Id = previewInstructionsAudioFileFileResponse.Value.Id;

            var questionsInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, vocabPart.QuestionsInstructionsAudioFileFile));
            if (!questionsInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(questionsInstructionsAudioFileFileResponse.Errors);
                return null;
            }

            vocabPart.QuestionsInstructionsAudioFileFile.Id = questionsInstructionsAudioFileFileResponse.Value.Id;

            vocabPart.PreviewInstructionsText = htmlSanitizer.Sanitize(vocabPart.PreviewInstructionsText);

            vocabPart.QuestionsInstructionsText = htmlSanitizer.Sanitize(vocabPart.QuestionsInstructionsText);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await VocabPartInsert.Execute(connection, transaction, request.SessionId, vocabPart);

                return new VocabPart
                {
                    Id = id,
                    AssessmentId = vocabPart.AssessmentId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<VocabPart> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var vocabPart = request.Value;

            var existing = await VocabPartSelect.Execute(Connection, request.SessionId, vocabPart);

            var previewInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, vocabPart.PreviewInstructionsAudioFileFile), existing?.PreviewInstructionsAudioFileFile);
            if (!previewInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(previewInstructionsAudioFileFileResponse.Errors);
                return;
            }

            vocabPart.PreviewInstructionsAudioFileFile.Id = previewInstructionsAudioFileFileResponse.Value.Id;

            var questionsInstructionsAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, vocabPart.QuestionsInstructionsAudioFileFile), existing?.QuestionsInstructionsAudioFileFile);
            if (!questionsInstructionsAudioFileFileResponse.Ok)
            {
                response.AddErrors(questionsInstructionsAudioFileFileResponse.Errors);
                return;
            }

            vocabPart.QuestionsInstructionsAudioFileFile.Id = questionsInstructionsAudioFileFileResponse.Value.Id;

            vocabPart.PreviewInstructionsText = htmlSanitizer.Sanitize(vocabPart.PreviewInstructionsText);

            vocabPart.QuestionsInstructionsText = htmlSanitizer.Sanitize(vocabPart.QuestionsInstructionsText);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await VocabPartUpdate.Execute(connection, transaction, request.SessionId, vocabPart);
            });
        });
    }

    public async Task<Response> Remove(Request<VocabPart> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var vocabPart = request.Value;
            var existing = await VocabPartSelect.Execute(Connection, request.SessionId, vocabPart);

            if (existing is null)
                return;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await VocabPartDelete.Execute(connection, transaction, request.SessionId, vocabPart);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<VocabPart>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var vocabParts = request.Value;

            vocabParts.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await VocabPartUpdateOrdinals.Execute(connection, transaction, request.SessionId, vocabParts);
            });
        });
    }
}
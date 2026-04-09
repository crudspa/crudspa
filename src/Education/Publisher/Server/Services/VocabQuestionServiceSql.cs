namespace Crudspa.Education.Publisher.Server.Services;

public class VocabQuestionServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService)
    : IVocabQuestionService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<VocabQuestion>>> FetchForVocabPart(Request<VocabPart> request)
    {
        return await wrappers.Try<IList<VocabQuestion>>(request, async response =>
        {
            var vocabQuestions = await VocabQuestionSelectForVocabPart.Execute(Connection, request.SessionId, request.Value.Id);

            return vocabQuestions;
        });
    }

    public async Task<Response<VocabQuestion?>> Fetch(Request<VocabQuestion> request)
    {
        return await wrappers.Try<VocabQuestion?>(request, async response =>
        {
            var vocabQuestion = await VocabQuestionSelect.Execute(Connection, request.SessionId, request.Value);

            return vocabQuestion;
        });
    }

    public async Task<Response<VocabQuestion?>> Add(Request<VocabQuestion> request)
    {
        return await wrappers.Validate<VocabQuestion?, VocabQuestion>(request, async response =>
        {
            var vocabQuestion = request.Value;

            var audioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, vocabQuestion.AudioFileFile));
            if (!audioFileFileResponse.Ok)
            {
                response.AddErrors(audioFileFileResponse.Errors);
                return null;
            }

            vocabQuestion.AudioFileFile.Id = audioFileFileResponse.Value.Id;

            foreach (var vocabChoice in vocabQuestion.VocabChoices)
            {
                var vocabChoiceAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, vocabChoice.AudioFileFile), vocabChoice.AudioFileFile.Id);
                if (!vocabChoiceAudioFileFileResponse.Ok)
                {
                    response.AddErrors(vocabChoiceAudioFileFileResponse.Errors);
                    return null;
                }

                if (vocabChoiceAudioFileFileResponse.Value is not null) vocabChoice.AudioFileFile = vocabChoiceAudioFileFileResponse.Value;
            }

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await VocabQuestionInsert.Execute(connection, transaction, request.SessionId, vocabQuestion);

                foreach (var vocabChoice in vocabQuestion.VocabChoices)
                {
                    vocabChoice.VocabQuestionId = id;
                    await VocabChoiceInsertByBatch.Execute(connection, transaction, request.SessionId, vocabChoice);
                }

                return new VocabQuestion
                {
                    Id = id,
                    VocabPartId = vocabQuestion.VocabPartId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<VocabQuestion> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var vocabQuestion = request.Value;

            var existing = await VocabQuestionSelect.Execute(Connection, request.SessionId, vocabQuestion);

            var audioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, vocabQuestion.AudioFileFile), existing?.AudioFileFile);
            if (!audioFileFileResponse.Ok)
            {
                response.AddErrors(audioFileFileResponse.Errors);
                return;
            }

            vocabQuestion.AudioFileFile.Id = audioFileFileResponse.Value.Id;

            foreach (var vocabChoice in vocabQuestion.VocabChoices)
            {
                var existingVocabChoice = existing?.VocabChoices.FirstOrDefault(x => x.Id.Equals(vocabChoice.Id));

                if (vocabChoice.AudioFileFile is not null)
                {
                    var vocabChoiceAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, vocabChoice.AudioFileFile), existingVocabChoice?.AudioFileFile?.Id);
                    if (!vocabChoiceAudioFileFileResponse.Ok)
                    {
                        response.AddErrors(vocabChoiceAudioFileFileResponse.Errors);
                        return;
                    }

                    if (vocabChoiceAudioFileFileResponse.Value is not null) vocabChoice.AudioFileFile = vocabChoiceAudioFileFileResponse.Value;
                }
            }

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await VocabQuestionUpdate.Execute(connection, transaction, request.SessionId, vocabQuestion);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.VocabChoices,
                    vocabQuestion.VocabChoices,
                    VocabChoiceInsertByBatch.Execute,
                    VocabChoiceUpdateByBatch.Execute,
                    VocabChoiceDeleteByBatch.Execute);

                vocabQuestion.VocabChoices.EnsureOrder();
                await VocabChoiceUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, vocabQuestion.VocabChoices);
            });
        });
    }

    public async Task<Response> Remove(Request<VocabQuestion> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var vocabQuestion = request.Value;
            var existing = await VocabQuestionSelect.Execute(Connection, request.SessionId, vocabQuestion);

            if (existing is null)
                return;

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                foreach (var vocabChoice in existing.VocabChoices)
                    await VocabChoiceDeleteByBatch.Execute(connection, transaction, request.SessionId, vocabChoice);

                await VocabQuestionDelete.Execute(connection, transaction, request.SessionId, vocabQuestion);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<VocabQuestion>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var vocabQuestions = request.Value;

            vocabQuestions.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await VocabQuestionUpdateOrdinals.Execute(connection, transaction, request.SessionId, vocabQuestions);
            });
        });
    }
}
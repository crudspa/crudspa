namespace Crudspa.Education.Publisher.Server.Services;

public class ReadQuestionServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IReadQuestionService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ReadQuestion>>> FetchForReadPart(Request<ReadPart> request)
    {
        return await wrappers.Try<IList<ReadQuestion>>(request, async response =>
        {
            var readQuestions = await ReadQuestionSelectForReadPart.Execute(Connection, request.SessionId, request.Value.Id);
            return readQuestions;
        });
    }

    public async Task<Response<ReadQuestion?>> Fetch(Request<ReadQuestion> request)
    {
        return await wrappers.Try<ReadQuestion?>(request, async response =>
        {
            var readQuestion = await ReadQuestionSelect.Execute(Connection, request.SessionId, request.Value);
            return readQuestion;
        });
    }

    public async Task<Response<ReadQuestion?>> Add(Request<ReadQuestion> request)
    {
        return await wrappers.Validate<ReadQuestion?, ReadQuestion>(request, async response =>
        {
            var readQuestion = request.Value;

            var audioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readQuestion.AudioFileFile));
            if (!audioFileFileResponse.Ok)
            {
                response.AddErrors(audioFileFileResponse.Errors);
                return null;
            }

            readQuestion.AudioFileFile.Id = audioFileFileResponse.Value.Id;

            var imageFileFileResponse = await fileService.SaveImage(new(request.SessionId, readQuestion.ImageFileFile));
            if (!imageFileFileResponse.Ok)
            {
                response.AddErrors(imageFileFileResponse.Errors);
                return null;
            }

            readQuestion.ImageFileFile.Id = imageFileFileResponse.Value.Id;

            foreach (var readChoice in readQuestion.ReadChoices)
            {
                var readChoiceImageFileFileResponse = await fileService.SaveImage(new(request.SessionId, readChoice.ImageFileFile), readChoice.ImageFileFile.Id);
                if (!readChoiceImageFileFileResponse.Ok)
                {
                    response.AddErrors(readChoiceImageFileFileResponse.Errors);
                    return null;
                }

                if (readChoiceImageFileFileResponse.Value is not null) readChoice.ImageFileFile = readChoiceImageFileFileResponse.Value;

                var readChoiceAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readChoice.AudioFileFile), readChoice.AudioFileFile.Id);
                if (!readChoiceAudioFileFileResponse.Ok)
                {
                    response.AddErrors(readChoiceAudioFileFileResponse.Errors);
                    return null;
                }

                if (readChoiceAudioFileFileResponse.Value is not null) readChoice.AudioFileFile = readChoiceAudioFileFileResponse.Value;
            }

            readQuestion.Text = htmlSanitizer.Sanitize(readQuestion.Text);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await ReadQuestionInsert.Execute(connection, transaction, request.SessionId, readQuestion);

                foreach (var readChoice in readQuestion.ReadChoices)
                {
                    readChoice.ReadQuestionId = id;
                    await ReadChoiceInsertByBatch.Execute(connection, transaction, request.SessionId, readChoice);
                }

                return new ReadQuestion
                {
                    Id = id,
                    ReadPartId = readQuestion.ReadPartId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ReadQuestion> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var readQuestion = request.Value;

            var existing = await ReadQuestionSelect.Execute(Connection, request.SessionId, readQuestion);

            var audioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readQuestion.AudioFileFile), existing?.AudioFileFile);
            if (!audioFileFileResponse.Ok)
            {
                response.AddErrors(audioFileFileResponse.Errors);
                return;
            }

            readQuestion.AudioFileFile.Id = audioFileFileResponse.Value.Id;

            var imageFileFileResponse = await fileService.SaveImage(new(request.SessionId, readQuestion.ImageFileFile), existing?.ImageFileFile);
            if (!imageFileFileResponse.Ok)
            {
                response.AddErrors(imageFileFileResponse.Errors);
                return;
            }

            readQuestion.ImageFileFile.Id = imageFileFileResponse.Value.Id;

            foreach (var readChoice in readQuestion.ReadChoices)
            {
                var existingReadChoice = existing?.ReadChoices.FirstOrDefault(x => x.Id.Equals(readChoice.Id));

                if (readChoice.ImageFileFile is not null)
                {
                    var readChoiceImageFileFileResponse = await fileService.SaveImage(new(request.SessionId, readChoice.ImageFileFile), existingReadChoice?.ImageFileFile?.Id);
                    if (!readChoiceImageFileFileResponse.Ok)
                    {
                        response.AddErrors(readChoiceImageFileFileResponse.Errors);
                        return;
                    }

                    if (readChoiceImageFileFileResponse.Value is not null) readChoice.ImageFileFile = readChoiceImageFileFileResponse.Value;
                }

                if (readChoice.AudioFileFile is not null)
                {
                    var readChoiceAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, readChoice.AudioFileFile), existingReadChoice?.AudioFileFile?.Id);
                    if (!readChoiceAudioFileFileResponse.Ok)
                    {
                        response.AddErrors(readChoiceAudioFileFileResponse.Errors);
                        return;
                    }

                    if (readChoiceAudioFileFileResponse.Value is not null) readChoice.AudioFileFile = readChoiceAudioFileFileResponse.Value;
                }
            }

            readQuestion.Text = htmlSanitizer.Sanitize(readQuestion.Text);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ReadQuestionUpdate.Execute(connection, transaction, request.SessionId, readQuestion);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.ReadChoices,
                    readQuestion.ReadChoices,
                    ReadChoiceInsertByBatch.Execute,
                    ReadChoiceUpdateByBatch.Execute,
                    ReadChoiceDeleteByBatch.Execute);

                readQuestion.ReadChoices.EnsureOrder();
                await ReadChoiceUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, readQuestion.ReadChoices);
            });
        });
    }

    public async Task<Response> Remove(Request<ReadQuestion> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var readQuestion = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ReadQuestionDelete.Execute(connection, transaction, request.SessionId, readQuestion);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<ReadQuestion>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var readQuestions = request.Value;

            readQuestions.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ReadQuestionUpdateOrdinals.Execute(connection, transaction, request.SessionId, readQuestions);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchReadQuestionCategoryNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ReadQuestionCategorySelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchReadQuestionTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ReadQuestionTypeSelectOrderables.Execute(Connection));
    }
}
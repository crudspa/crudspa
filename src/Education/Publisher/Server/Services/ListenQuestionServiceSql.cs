namespace Crudspa.Education.Publisher.Server.Services;

public class ListenQuestionServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IListenQuestionService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<ListenQuestion>>> FetchForListenPart(Request<ListenPart> request)
    {
        return await wrappers.Try<IList<ListenQuestion>>(request, async response =>
        {
            var listenQuestions = await ListenQuestionSelectForListenPart.Execute(Connection, request.SessionId, request.Value.Id);
            return listenQuestions;
        });
    }

    public async Task<Response<ListenQuestion?>> Fetch(Request<ListenQuestion> request)
    {
        return await wrappers.Try<ListenQuestion?>(request, async response =>
        {
            var listenQuestion = await ListenQuestionSelect.Execute(Connection, request.SessionId, request.Value);
            return listenQuestion;
        });
    }

    public async Task<Response<ListenQuestion?>> Add(Request<ListenQuestion> request)
    {
        return await wrappers.Validate<ListenQuestion?, ListenQuestion>(request, async response =>
        {
            var listenQuestion = request.Value;

            var audioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenQuestion.AudioFileFile));
            if (!audioFileFileResponse.Ok)
            {
                response.AddErrors(audioFileFileResponse.Errors);
                return null;
            }

            listenQuestion.AudioFileFile.Id = audioFileFileResponse.Value.Id;

            var imageFileFileResponse = await fileService.SaveImage(new(request.SessionId, listenQuestion.ImageFileFile));
            if (!imageFileFileResponse.Ok)
            {
                response.AddErrors(imageFileFileResponse.Errors);
                return null;
            }

            listenQuestion.ImageFileFile.Id = imageFileFileResponse.Value.Id;

            foreach (var listenChoice in listenQuestion.ListenChoices)
            {
                var listenChoiceImageFileFileResponse = await fileService.SaveImage(new(request.SessionId, listenChoice.ImageFileFile), listenChoice.ImageFileFile.Id);
                if (!listenChoiceImageFileFileResponse.Ok)
                {
                    response.AddErrors(listenChoiceImageFileFileResponse.Errors);
                    return null;
                }

                if (listenChoiceImageFileFileResponse.Value is not null) listenChoice.ImageFileFile = listenChoiceImageFileFileResponse.Value;

                var listenChoiceAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenChoice.AudioFileFile), listenChoice.AudioFileFile.Id);
                if (!listenChoiceAudioFileFileResponse.Ok)
                {
                    response.AddErrors(listenChoiceAudioFileFileResponse.Errors);
                    return null;
                }

                if (listenChoiceAudioFileFileResponse.Value is not null) listenChoice.AudioFileFile = listenChoiceAudioFileFileResponse.Value;
            }

            listenQuestion.Text = htmlSanitizer.Sanitize(listenQuestion.Text);

            return await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                var id = await ListenQuestionInsert.Execute(connection, transaction, request.SessionId, listenQuestion);

                foreach (var listenChoice in listenQuestion.ListenChoices)
                {
                    listenChoice.ListenQuestionId = id;
                    await ListenChoiceInsertByBatch.Execute(connection, transaction, request.SessionId, listenChoice);
                }

                return new ListenQuestion
                {
                    Id = id,
                    ListenPartId = listenQuestion.ListenPartId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<ListenQuestion> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var listenQuestion = request.Value;

            var existing = await ListenQuestionSelect.Execute(Connection, request.SessionId, listenQuestion);

            var audioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenQuestion.AudioFileFile), existing?.AudioFileFile);
            if (!audioFileFileResponse.Ok)
            {
                response.AddErrors(audioFileFileResponse.Errors);
                return;
            }

            listenQuestion.AudioFileFile.Id = audioFileFileResponse.Value.Id;

            var imageFileFileResponse = await fileService.SaveImage(new(request.SessionId, listenQuestion.ImageFileFile), existing?.ImageFileFile);
            if (!imageFileFileResponse.Ok)
            {
                response.AddErrors(imageFileFileResponse.Errors);
                return;
            }

            listenQuestion.ImageFileFile.Id = imageFileFileResponse.Value.Id;

            foreach (var listenChoice in listenQuestion.ListenChoices)
            {
                var existingListenChoice = existing?.ListenChoices.FirstOrDefault(x => x.Id.Equals(listenChoice.Id));

                if (listenChoice.ImageFileFile.Id is not null)
                {
                    var listenChoiceImageFileFileResponse = await fileService.SaveImage(new(request.SessionId, listenChoice.ImageFileFile), existingListenChoice?.ImageFileFile?.Id);
                    if (!listenChoiceImageFileFileResponse.Ok)
                    {
                        response.AddErrors(listenChoiceImageFileFileResponse.Errors);
                        return;
                    }

                    if (listenChoiceImageFileFileResponse.Value is not null) listenChoice.ImageFileFile = listenChoiceImageFileFileResponse.Value;
                }

                if (listenChoice.AudioFileFile.Id is not null)
                {
                    var listenChoiceAudioFileFileResponse = await fileService.SaveAudio(new(request.SessionId, listenChoice.AudioFileFile), existingListenChoice?.AudioFileFile?.Id);
                    if (!listenChoiceAudioFileFileResponse.Ok)
                    {
                        response.AddErrors(listenChoiceAudioFileFileResponse.Errors);
                        return;
                    }

                    if (listenChoiceAudioFileFileResponse.Value is not null) listenChoice.AudioFileFile = listenChoiceAudioFileFileResponse.Value;
                }
            }

            listenQuestion.Text = htmlSanitizer.Sanitize(listenQuestion.Text);

            await sqlWrappers.WithTransaction(async (connection, transaction) =>
            {
                await ListenQuestionUpdate.Execute(connection, transaction, request.SessionId, listenQuestion);

                await SqlWrappersCore.MergeBatch(connection, transaction, request.SessionId,
                    existing!.ListenChoices,
                    listenQuestion.ListenChoices,
                    ListenChoiceInsertByBatch.Execute,
                    ListenChoiceUpdateByBatch.Execute,
                    ListenChoiceDeleteByBatch.Execute);

                listenQuestion.ListenChoices.EnsureOrder();
                await ListenChoiceUpdateOrdinalsByBatch.Execute(connection, transaction, request.SessionId, listenQuestion.ListenChoices);
            });
        });
    }

    public async Task<Response> Remove(Request<ListenQuestion> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var listenQuestion = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ListenQuestionDelete.Execute(connection, transaction, request.SessionId, listenQuestion);
            });
        });
    }

    public async Task<Response> SaveOrder(Request<IList<ListenQuestion>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var listenQuestions = request.Value;

            listenQuestions.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await ListenQuestionUpdateOrdinals.Execute(connection, transaction, request.SessionId, listenQuestions);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchListenQuestionCategoryNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ListenQuestionCategorySelectOrderables.Execute(Connection));
    }

    public async Task<Response<IList<Orderable>>> FetchReadQuestionTypeNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ReadQuestionTypeSelectOrderables.Execute(Connection));
    }
}
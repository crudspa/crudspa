namespace Crudspa.Content.Design.Server.Services;

public class SurveyServiceSql(
    IServiceWrappers wrappers,
    ISqlWrappers sqlWrappers,
    IServerConfigService configService,
    IHtmlSanitizer htmlSanitizer)
    : ISurveyService
{
    private String Connection => configService.Fetch().Database;

    public async Task<Response<IList<Named>>> FetchNames(Request<Portal> request)
    {
        return await wrappers.Try<IList<Named>>(request, async response =>
            await SurveySelectNames.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<IList<Survey>>> FetchForPortal(Request<Portal> request)
    {
        return await wrappers.Try<IList<Survey>>(request, async response =>
            await SurveySelectForPortal.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<Survey?>> Fetch(Request<Survey> request)
    {
        return await wrappers.Try<Survey?>(request, async response =>
            await SurveySelect.Execute(Connection, request.SessionId, request.Value));
    }

    public async Task<Response<Survey?>> Add(Request<Survey> request)
    {
        return await wrappers.Validate<Survey?, Survey>(request, async response =>
        {
            var survey = request.Value;

            survey.StatusId ??= Crudspa.Framework.Core.Shared.Contracts.Ids.ContentStatusIds.Draft;
            survey.Description = htmlSanitizer.Sanitize(survey.Description);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await SurveyInsert.Execute(connection, transaction, request.SessionId, survey);

                return new Survey
                {
                    Id = id,
                    PortalId = survey.PortalId,
                };
            });
        });
    }

    public async Task<Response> Save(Request<Survey> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var survey = request.Value;

            survey.Description = htmlSanitizer.Sanitize(survey.Description);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyUpdate.Execute(connection, transaction, request.SessionId, survey);
            });
        });
    }

    public async Task<Response> Remove(Request<Survey> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyDelete.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }

    public async Task<Response<IList<Orderable>>> FetchContentStatusNames(Request request)
    {
        return await wrappers.Try<IList<Orderable>>(request, async response =>
            await ContentStatusSelectOrderables.Execute(Connection, request.SessionId));
    }

    public async Task<Response<IList<SurveyPart>>> FetchParts(Request<Survey> request)
    {
        return await wrappers.Try<IList<SurveyPart>>(request, async response =>
            await SurveyPartSelectForSurvey.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<SurveyPart?>> FetchPart(Request<SurveyPart> request)
    {
        return await wrappers.Try<SurveyPart?>(request, async response =>
            await SurveyPartSelect.Execute(Connection, request.SessionId, request.Value));
    }

    public async Task<Response<SurveyPart?>> AddPart(Request<SurveyPart> request)
    {
        return await wrappers.Validate<SurveyPart?, SurveyPart>(request, async response =>
        {
            var part = request.Value;

            part.Instructions = htmlSanitizer.Sanitize(part.Instructions);

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                var id = await SurveyPartInsert.Execute(connection, transaction, request.SessionId, part);

                return new SurveyPart
                {
                    Id = id,
                    SurveyId = part.SurveyId,
                };
            });
        });
    }

    public async Task<Response> SavePart(Request<SurveyPart> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var part = request.Value;

            part.Instructions = htmlSanitizer.Sanitize(part.Instructions);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyPartUpdate.Execute(connection, transaction, request.SessionId, part);
            });
        });
    }

    public async Task<Response> RemovePart(Request<SurveyPart> request)
    {
        return await wrappers.Try(request, async response =>
        {
            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyPartDelete.Execute(connection, transaction, request.SessionId, request.Value);
            });
        });
    }

    public async Task<Response> SavePartOrder(Request<IList<SurveyPart>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var parts = request.Value;

            parts.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyPartUpdateOrdinals.Execute(connection, transaction, request.SessionId, parts);
            });
        });
    }

    public async Task<Response<IList<SurveyQuestion>>> FetchQuestions(Request<SurveyPart> request)
    {
        return await wrappers.Try<IList<SurveyQuestion>>(request, async response =>
            await SurveyQuestionSelectForPart.Execute(Connection, request.SessionId, request.Value.Id));
    }

    public async Task<Response<SurveyQuestion?>> FetchQuestion(Request<SurveyQuestion> request)
    {
        return await wrappers.Try<SurveyQuestion?>(request, async response =>
            await SurveyQuestionSelect.Execute(Connection, request.SessionId, request.Value));
    }

    public async Task<Response<SurveyQuestion?>> AddQuestion(Request<SurveyQuestion> request)
    {
        return await wrappers.Validate<SurveyQuestion?, SurveyQuestion>(request, async response =>
        {
            var surveyQuestion = request.Value;

            return await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SaveQuestionData(connection, transaction, request.SessionId, surveyQuestion.Question);

                surveyQuestion.QuestionId = surveyQuestion.Question.Id;
                var id = await SurveyQuestionInsert.Execute(connection, transaction, request.SessionId, surveyQuestion);

                return new SurveyQuestion
                {
                    Id = id,
                    PartId = surveyQuestion.PartId,
                };
            });
        });
    }

    public async Task<Response> SaveQuestion(Request<SurveyQuestion> request)
    {
        return await wrappers.Validate(request, async response =>
        {
            var surveyQuestion = request.Value;

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SaveQuestionData(connection, transaction, request.SessionId, surveyQuestion.Question);

                surveyQuestion.QuestionId = surveyQuestion.Question.Id;
                await SurveyQuestionUpdate.Execute(connection, transaction, request.SessionId, surveyQuestion);
            });
        });
    }

    public async Task<Response> RemoveQuestion(Request<SurveyQuestion> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var surveyQuestion = await SurveyQuestionSelect.Execute(Connection, request.SessionId, request.Value);

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyQuestionDelete.Execute(connection, transaction, request.SessionId, request.Value);

                if (surveyQuestion?.QuestionId.HasValue == true)
                    await QuestionDelete.Execute(connection, transaction, request.SessionId, surveyQuestion.QuestionId);
            });
        });
    }

    public async Task<Response> SaveQuestionOrder(Request<IList<SurveyQuestion>> request)
    {
        return await wrappers.Try(request, async response =>
        {
            var questions = request.Value;

            questions.EnsureOrder();

            await sqlWrappers.WithConnection(async (connection, transaction) =>
            {
                await SurveyQuestionUpdateOrdinals.Execute(connection, transaction, request.SessionId, questions);
            });
        });
    }

    private async Task SaveQuestionData(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Question question)
    {
        question.Text = htmlSanitizer.Sanitize(question.Text);
        question.EnsureAnswer();

        if (question.Id.HasValue)
            await QuestionUpdate.Execute(connection, transaction, sessionId, question);
        else
            question.Id = await QuestionInsert.Execute(connection, transaction, sessionId, question);

        await SaveAnswers(connection, transaction, sessionId, question);
    }

    private async Task SaveAnswers(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Question question)
    {
        question.SetQuestionIds();

        if (question.BooleanAnswer is { } booleanAnswer)
        {
            booleanAnswer.QuestionId = question.Id;
            if (booleanAnswer.Id.HasValue)
                await BooleanAnswerUpdate.Execute(connection, transaction, sessionId, booleanAnswer);
            else
                booleanAnswer.Id = await BooleanAnswerInsert.Execute(connection, transaction, sessionId, booleanAnswer);
        }

        if (question.ContactAnswer is { } contactAnswer)
        {
            contactAnswer.QuestionId = question.Id;
            if (contactAnswer.Id.HasValue)
                await ContactAnswerUpdate.Execute(connection, transaction, sessionId, contactAnswer);
            else
                contactAnswer.Id = await ContactAnswerInsert.Execute(connection, transaction, sessionId, contactAnswer);
        }

        if (question.DateAnswer is { } dateAnswer)
        {
            dateAnswer.QuestionId = question.Id;
            if (dateAnswer.Id.HasValue)
                await DateAnswerUpdate.Execute(connection, transaction, sessionId, dateAnswer);
            else
                dateAnswer.Id = await DateAnswerInsert.Execute(connection, transaction, sessionId, dateAnswer);
        }

        if (question.FileAnswer is { } fileAnswer)
        {
            fileAnswer.QuestionId = question.Id;
            if (fileAnswer.Id.HasValue)
                await FileAnswerUpdate.Execute(connection, transaction, sessionId, fileAnswer);
            else
                fileAnswer.Id = await FileAnswerInsert.Execute(connection, transaction, sessionId, fileAnswer);
        }

        if (question.NumberAnswer is { } numberAnswer)
        {
            numberAnswer.QuestionId = question.Id;
            if (numberAnswer.Id.HasValue)
                await NumberAnswerUpdate.Execute(connection, transaction, sessionId, numberAnswer);
            else
                numberAnswer.Id = await NumberAnswerInsert.Execute(connection, transaction, sessionId, numberAnswer);
        }

        if (question.OptionsAnswer is { } optionsAnswer)
        {
            optionsAnswer.QuestionId = question.Id;
            if (optionsAnswer.Id.HasValue)
                await OptionsAnswerUpdate.Execute(connection, transaction, sessionId, optionsAnswer);
            else
                optionsAnswer.Id = await OptionsAnswerInsert.Execute(connection, transaction, sessionId, optionsAnswer);

            await SaveOptionsChoices(connection, transaction, sessionId, optionsAnswer);
        }

        if (question.ScaleAnswer is { } scaleAnswer)
        {
            scaleAnswer.QuestionId = question.Id;
            if (scaleAnswer.Id.HasValue)
                await ScaleAnswerUpdate.Execute(connection, transaction, sessionId, scaleAnswer);
            else
                scaleAnswer.Id = await ScaleAnswerInsert.Execute(connection, transaction, sessionId, scaleAnswer);
        }

        if (question.TextAnswer is { } textAnswer)
        {
            textAnswer.QuestionId = question.Id;
            if (textAnswer.Id.HasValue)
                await TextAnswerUpdate.Execute(connection, transaction, sessionId, textAnswer);
            else
                textAnswer.Id = await TextAnswerInsert.Execute(connection, transaction, sessionId, textAnswer);
        }
    }

    private async Task SaveOptionsChoices(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, OptionsAnswer optionsAnswer)
    {
        foreach (var choice in optionsAnswer.Choices)
        {
            choice.OptionsAnswerId = optionsAnswer.Id;
            choice.Text = htmlSanitizer.Sanitize(choice.Text);
        }

        optionsAnswer.Choices.EnsureOrder();

        var existingChoices = optionsAnswer.Id.HasValue
            ? await OptionsAnswerChoiceSelectForOptionsAnswer.Execute(Connection, optionsAnswer.Id)
            : [];

        await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
            existingChoices,
            optionsAnswer.Choices,
            OptionsAnswerChoiceInsertByBatch.Execute,
            OptionsAnswerChoiceUpdateByBatch.Execute,
            OptionsAnswerChoiceDeleteByBatch.Execute);
    }
}
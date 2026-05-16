namespace Crudspa.Content.Design.Server.Services;

public class ElementRepositoryQuestion(
    IServerConfigService configService,
    IFileService fileService,
    IHtmlSanitizer htmlSanitizer)
    : IElementRepository
{
    public Task<SectionElement> Create(ElementType elementType, Guid? sectionId, Int32? ordinal)
    {
        var elementId = Guid.NewGuid();
        var questionId = Guid.NewGuid();

        var sectionElement = new SectionElement
        {
            Element = new()
            {
                Id = elementId,
                SectionId = sectionId,
                TypeId = elementType.Id,
                ElementType = elementType,
                RequireInteraction = true,
                Ordinal = ordinal,
                Item = new()
                {
                    BasisId = BasisIds.Auto,
                    Grow = "1",
                    Shrink = "1",
                    AlignSelfId = AlignSelfIds.Stretch,
                },
            },
        };

        sectionElement.SetConfig(new QuestionElement
        {
            Id = Guid.NewGuid(),
            ElementId = elementId,
            QuestionId = questionId,
            Question = new()
            {
                Id = questionId,
                AnswerTypeId = AnswerTypeIds.Text,
                TextAnswer = new()
                {
                    Id = Guid.NewGuid(),
                    QuestionId = questionId,
                },
            },
        });

        return Task.FromResult(sectionElement);
    }

    public async Task<IList<Error>> Validate(String connection, SectionElement element)
    {
        var questionElement = element.RequireConfig<QuestionElement>();

        return await ErrorsEx.Validate(errors =>
        {
            errors.AddRange(element.Element.Validate());
            errors.AddRange(questionElement.Validate());
            return Task.CompletedTask;
        });
    }

    public async Task<Guid?> Insert(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var questionElement = element.RequireConfig<QuestionElement>();

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile));
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        var elementId = await ElementInsert.Execute(connection, transaction, sessionId, element.Element);

        element.ElementId = elementId;
        questionElement.ElementId = elementId;

        await InsertQuestion(connection, transaction, sessionId, questionElement.Question);

        questionElement.QuestionId = questionElement.Question.Id;
        questionElement.Id = await QuestionElementInsert.Execute(connection, transaction, sessionId, questionElement);

        return elementId;
    }

    public async Task Update(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var questionElement = element.RequireConfig<QuestionElement>();

        var existingElementBox = await BoxSelect.Execute(configService.Fetch().Database, sessionId, element.Box);

        var backgroundImageResponse = await fileService.SaveImage(new(sessionId, element.Box.BackgroundImageFile), existingElementBox?.BackgroundImageFile);
        if (!backgroundImageResponse.Ok)
            throw new("Call to IFileService.SaveImage() failed. " + backgroundImageResponse.ErrorMessages);
        element.Box.BackgroundImageFile.Id = backgroundImageResponse.Value!.Id;

        element.Box.Id = await BoxUpsert.Execute(connection, transaction, sessionId, element.Box);
        element.Item.Id = await ItemUpsert.Execute(connection, transaction, sessionId, element.Item);

        await ElementUpdate.Execute(connection, transaction, sessionId, element.Element);

        await SaveQuestion(connection, transaction, sessionId, questionElement.Question);

        questionElement.ElementId = element.Id;
        questionElement.QuestionId = questionElement.Question.Id;
        await QuestionElementUpdate.Execute(connection, transaction, sessionId, questionElement);
    }

    public async Task Delete(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, SectionElement element)
    {
        var questionElement = element.RequireConfig<QuestionElement>();

        await ElementDelete.Execute(connection, transaction, sessionId, element.ElementId);
        await QuestionElementDelete.Execute(connection, transaction, sessionId, questionElement);
        await QuestionDelete.Execute(connection, transaction, sessionId, questionElement.QuestionId);
    }

    private async Task InsertQuestion(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Question question)
    {
        question.Text = htmlSanitizer.Sanitize(question.Text);
        question.EnsureAnswer();
        question.Id = await QuestionInsert.Execute(connection, transaction, sessionId, question);

        await SaveAnswers(connection, transaction, sessionId, question);
    }

    private async Task SaveQuestion(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Question question)
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
            ? await OptionsAnswerChoiceSelectForOptionsAnswer.Execute(configService.Fetch().Database, optionsAnswer.Id)
            : [];

        await SqlWrappersCore.MergeBatch(connection, transaction, sessionId,
            existingChoices,
            optionsAnswer.Choices,
            OptionsAnswerChoiceInsertByBatch.Execute,
            OptionsAnswerChoiceUpdateByBatch.Execute,
            OptionsAnswerChoiceDeleteByBatch.Execute);
    }
}
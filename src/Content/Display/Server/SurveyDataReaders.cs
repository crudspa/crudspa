namespace Crudspa.Content.Display.Server;

public static class SurveyDataReaders
{
    public static async Task<Survey?> ReadSurveyWithQuestions(SqlDataReader reader)
    {
        Survey? survey = null;

        if (await reader.ReadAsync())
            survey = ReadSurvey(reader);

        await reader.NextResultAsync();
        var parts = new List<SurveyPart>();
        while (await reader.ReadAsync())
            parts.Add(ReadSurveyPart(reader));

        await reader.NextResultAsync();
        var questions = new List<SurveyQuestion>();
        while (await reader.ReadAsync())
            questions.Add(ReadSurveyQuestion(reader));

        await ReadAnswers(reader, questions);

        await reader.NextResultAsync();
        SurveyReply? reply = null;
        if (await reader.ReadAsync())
            reply = ReadSurveyReply(reader);

        await reader.NextResultAsync();
        var replies = new List<QuestionReply>();
        while (await reader.ReadAsync())
            replies.Add(ReadQuestionReply(reader));

        await reader.NextResultAsync();
        var choiceReplies = new List<AnswerChoiceReply>();
        while (await reader.ReadAsync())
            choiceReplies.Add(ReadAnswerChoiceReply(reader));

        var partsBySurveyId = parts.ToLookup(x => x.SurveyId);
        var questionsByPartId = questions.ToLookup(x => x.PartId);
        var choiceRepliesByReplyId = choiceReplies.ToLookup(x => x.QuestionReplyId);

        foreach (var questionReply in replies)
            questionReply.AnswerChoices = choiceRepliesByReplyId[questionReply.Id].ToObservable();

        if (reply is not null)
            reply.QuestionReplies = replies.ToObservable();

        foreach (var part in parts)
            part.Questions = questionsByPartId[part.Id].OrderBy(x => x.Ordinal).ToObservable();

        if (survey is not null)
        {
            survey.Parts = partsBySurveyId[survey.Id].OrderBy(x => x.Ordinal).ToObservable();
            survey.Reply = reply;
        }

        return survey;
    }

    public static async Task<IList<SurveyQuestion>> ReadSurveyQuestionsWithAnswers(SqlDataReader reader)
    {
        var questions = new List<SurveyQuestion>();

        while (await reader.ReadAsync())
            questions.Add(ReadSurveyQuestion(reader));

        await ReadAnswers(reader, questions);

        return questions;
    }

    public static async Task<QuestionReply?> ReadQuestionReplyWithChoices(SqlDataReader reader)
    {
        QuestionReply? reply = null;
        if (await reader.ReadAsync())
            reply = ReadQuestionReply(reader);

        await reader.NextResultAsync();
        var choiceReplies = new List<AnswerChoiceReply>();
        while (await reader.ReadAsync())
            choiceReplies.Add(ReadAnswerChoiceReply(reader));

        if (reply is not null)
            reply.AnswerChoices = choiceReplies.ToObservable();

        return reply;
    }

    private static async Task ReadAnswers(SqlDataReader reader, IList<SurveyQuestion> questions)
    {
        await reader.NextResultAsync();
        var booleanAnswers = new List<BooleanAnswer>();
        while (await reader.ReadAsync())
            booleanAnswers.Add(PageDataReaders.ReadBooleanAnswer(reader));

        await reader.NextResultAsync();
        var contactAnswers = new List<ContactAnswer>();
        while (await reader.ReadAsync())
            contactAnswers.Add(PageDataReaders.ReadContactAnswer(reader));

        await reader.NextResultAsync();
        var dateAnswers = new List<DateAnswer>();
        while (await reader.ReadAsync())
            dateAnswers.Add(PageDataReaders.ReadDateAnswer(reader));

        await reader.NextResultAsync();
        var fileAnswers = new List<FileAnswer>();
        while (await reader.ReadAsync())
            fileAnswers.Add(PageDataReaders.ReadFileAnswer(reader));

        await reader.NextResultAsync();
        var numberAnswers = new List<NumberAnswer>();
        while (await reader.ReadAsync())
            numberAnswers.Add(PageDataReaders.ReadNumberAnswer(reader));

        await reader.NextResultAsync();
        var optionsAnswers = new List<OptionsAnswer>();
        while (await reader.ReadAsync())
            optionsAnswers.Add(PageDataReaders.ReadOptionsAnswer(reader));

        await reader.NextResultAsync();
        var optionsAnswerChoices = new List<OptionsAnswerChoice>();
        while (await reader.ReadAsync())
            optionsAnswerChoices.Add(PageDataReaders.ReadOptionsAnswerChoice(reader));

        await reader.NextResultAsync();
        var scaleAnswers = new List<ScaleAnswer>();
        while (await reader.ReadAsync())
            scaleAnswers.Add(PageDataReaders.ReadScaleAnswer(reader));

        await reader.NextResultAsync();
        var textAnswers = new List<TextAnswer>();
        while (await reader.ReadAsync())
            textAnswers.Add(PageDataReaders.ReadTextAnswer(reader));

        var booleanAnswersByQuestionId = booleanAnswers.ToLookup(x => x.QuestionId);
        var contactAnswersByQuestionId = contactAnswers.ToLookup(x => x.QuestionId);
        var dateAnswersByQuestionId = dateAnswers.ToLookup(x => x.QuestionId);
        var fileAnswersByQuestionId = fileAnswers.ToLookup(x => x.QuestionId);
        var numberAnswersByQuestionId = numberAnswers.ToLookup(x => x.QuestionId);
        var optionsAnswersByQuestionId = optionsAnswers.ToLookup(x => x.QuestionId);
        var optionsAnswerChoicesByOptionsAnswerId = optionsAnswerChoices.ToLookup(x => x.OptionsAnswerId);
        var scaleAnswersByQuestionId = scaleAnswers.ToLookup(x => x.QuestionId);
        var textAnswersByQuestionId = textAnswers.ToLookup(x => x.QuestionId);

        foreach (var surveyQuestion in questions)
        {
            var question = surveyQuestion.Question;

            question.BooleanAnswer = booleanAnswersByQuestionId[question.Id].FirstOrDefault();
            question.ContactAnswer = contactAnswersByQuestionId[question.Id].FirstOrDefault();
            question.DateAnswer = dateAnswersByQuestionId[question.Id].FirstOrDefault();
            question.FileAnswer = fileAnswersByQuestionId[question.Id].FirstOrDefault();
            question.NumberAnswer = numberAnswersByQuestionId[question.Id].FirstOrDefault();
            question.OptionsAnswer = optionsAnswersByQuestionId[question.Id].FirstOrDefault();

            if (question.OptionsAnswer is not null)
                question.OptionsAnswer.Choices = optionsAnswerChoicesByOptionsAnswerId[question.OptionsAnswer.Id]
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();

            question.ScaleAnswer = scaleAnswersByQuestionId[question.Id].FirstOrDefault();
            question.TextAnswer = textAnswersByQuestionId[question.Id].FirstOrDefault();
            question.EnsureAnswer();
        }
    }

    public static Survey ReadSurvey(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            PortalId = reader.ReadGuid(1),
            PortalKey = reader.ReadString(2),
            Title = reader.ReadString(3),
            Description = reader.ReadString(4),
            StatusId = reader.ReadGuid(5),
            StatusName = reader.ReadString(6),
            AssignmentKind = reader.ReadEnum<Survey.AssignmentKinds>(7),
            PartCount = reader.ReadInt32(8),
        };
    }

    public static SurveyPart ReadSurveyPart(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            SurveyId = reader.ReadGuid(1),
            SurveyTitle = reader.ReadString(2),
            Title = reader.ReadString(3),
            Instructions = reader.ReadString(4),
            Ordinal = reader.ReadInt32(5),
            QuestionCount = reader.ReadInt32(6),
        };
    }

    public static SurveyQuestion ReadSurveyQuestion(SqlDataReader reader)
    {
        var questionId = reader.ReadGuid(3);

        return new()
        {
            Id = reader.ReadGuid(0),
            PartId = reader.ReadGuid(1),
            PartTitle = reader.ReadString(2),
            QuestionId = questionId,
            Question = new()
            {
                Id = questionId,
                Text = reader.ReadString(4),
                AnswerTypeId = reader.ReadGuid(5),
                AnswerType = new()
                {
                    Id = reader.ReadGuid(5),
                    Name = reader.ReadString(6),
                    DesignView = reader.ReadString(7),
                    DisplayView = reader.ReadString(8),
                },
            },
            Ordinal = reader.ReadInt32(9),
        };
    }

    public static SurveyReply ReadSurveyReply(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            SurveyId = reader.ReadGuid(1),
            BinderId = reader.ReadGuid(2),
            ContactId = reader.ReadGuid(3),
            Started = reader.ReadDateTimeOffset(4),
            Completed = reader.ReadDateTimeOffset(5),
            Terminated = reader.ReadDateTimeOffset(6),
        };
    }

    public static QuestionReply ReadQuestionReply(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            SurveyReplyId = reader.ReadGuid(1),
            QuestionId = reader.ReadGuid(2),
            Submitted = reader.ReadDateTimeOffset(3),
            BoolValue = reader.ReadBoolean(4),
            TextValue = reader.ReadString(5),
            HtmlValue = reader.ReadString(6),
            DateValue = reader.ReadDateOnly(7),
            TimeValue = reader.ReadTimeOnly(8),
            DateTimeValue = reader.ReadDateTimeOffset(9),
            IntegerValue = reader.ReadInt32(10),
            DecimalValue = reader.ReadSingle(11),
            CurrencyValue = reader.ReadSingle(12),
            OtherBoolValue = reader.ReadBoolean(13),
            OtherTextValue = reader.ReadString(14),
            AudioId = reader.ReadGuid(15),
            ImageId = reader.ReadGuid(16),
            PdfId = reader.ReadGuid(17),
            VideoId = reader.ReadGuid(18),
            PostalId = reader.ReadGuid(19),
            AudioFile = new() { Id = reader.ReadGuid(15) },
            ImageFile = new() { Id = reader.ReadGuid(16) },
            PdfFile = new() { Id = reader.ReadGuid(17) },
            VideoFile = new() { Id = reader.ReadGuid(18) },
            Postal = new()
            {
                Id = reader.ReadGuid(19),
                RecipientName = reader.ReadString(20),
                BusinessName = reader.ReadString(21),
                StreetAddress = reader.ReadString(22),
                City = reader.ReadString(23),
                StateId = reader.ReadGuid(24),
                PostalCode = reader.ReadString(25),
            },
        };
    }

    public static AnswerChoiceReply ReadAnswerChoiceReply(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            QuestionReplyId = reader.ReadGuid(1),
            ChoiceId = reader.ReadGuid(2),
        };
    }
}
namespace Crudspa.Education.Common.Server.Sproxies;

public static class AssessmentSelectProgress
{
    public static async Task<Assessment?> Execute(String connection, Guid? assessmentAssignmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationCommon.AssessmentSelectProgress";

        command.AddParameter("@AssessmentAssignmentId", assessmentAssignmentId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            var assignment = ReadAssessmentAssignment(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                assignment.ListenPartCompleteds.Add(ReadListenPartCompleted(reader));

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                assignment.ReadPartCompleteds.Add(ReadReadPartCompleted(reader));

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                assignment.VocabPartCompleteds.Add(ReadVocabPartCompleted(reader));

            await reader.NextResultAsync();

            var listenParts = new List<ListenPart>();

            while (await reader.ReadAsync())
                listenParts.Add(ReadListenPart(reader));

            await reader.NextResultAsync();

            var readParts = new List<ReadPart>();

            while (await reader.ReadAsync())
                readParts.Add(ReadReadPart(reader));

            await reader.NextResultAsync();

            var vocabParts = new List<VocabPart>();

            while (await reader.ReadAsync())
                vocabParts.Add(ReadVocabPart(reader));

            await reader.NextResultAsync();

            var listenQuestions = new List<ListenQuestion>();

            while (await reader.ReadAsync())
                listenQuestions.Add(ReadListenQuestion(reader));

            await reader.NextResultAsync();

            var listenChoices = new List<ListenChoice>();

            while (await reader.ReadAsync())
                listenChoices.Add(ReadListenChoice(reader));

            await reader.NextResultAsync();

            var listenChoiceSelections = new List<ListenChoiceSelection>();

            while (await reader.ReadAsync())
                listenChoiceSelections.Add(ReadListenChoiceSelection(reader));

            await reader.NextResultAsync();

            var readParagraphs = new List<ReadParagraph>();

            while (await reader.ReadAsync())
                readParagraphs.Add(ReadReadParagraph(reader));

            await reader.NextResultAsync();

            var readQuestions = new List<ReadQuestion>();

            while (await reader.ReadAsync())
                readQuestions.Add(ReadReadQuestion(reader));

            await reader.NextResultAsync();

            var readChoices = new List<ReadChoice>();

            while (await reader.ReadAsync())
                readChoices.Add(ReadReadChoice(reader));

            await reader.NextResultAsync();

            var readChoiceSelections = new List<ReadChoiceSelection>();

            while (await reader.ReadAsync())
                readChoiceSelections.Add(ReadReadChoiceSelection(reader));

            await reader.NextResultAsync();

            var vocabQuestions = new List<VocabQuestion>();

            while (await reader.ReadAsync())
                vocabQuestions.Add(ReadVocabQuestion(reader));

            await reader.NextResultAsync();

            var vocabChoices = new List<VocabChoice>();

            while (await reader.ReadAsync())
                vocabChoices.Add(ReadVocabChoice(reader));

            await reader.NextResultAsync();

            var vocabChoiceSelections = new List<VocabChoiceSelection>();

            while (await reader.ReadAsync())
                vocabChoiceSelections.Add(ReadVocabChoiceSelection(reader));

            foreach (var listenQuestion in listenQuestions)
            {
                listenQuestion.ListenChoices = listenChoices
                    .Where(x => x.ListenQuestionId.Equals(listenQuestion.Id))
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();

                listenQuestion.Selections = listenChoiceSelections
                    .Where(x => x.ChoiceListenQuestionId.Equals(listenQuestion.Id))
                    .OrderByDescending(x => x.Made)
                    .Take(1)
                    .ToObservable();
            }

            foreach (var listenPart in listenParts)
                listenPart.ListenQuestions = listenQuestions
                    .Where(x => x.ListenPartId.Equals(listenPart.Id))
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();

            foreach (var readQuestion in readQuestions)
            {
                readQuestion.ReadChoices = readChoices
                    .Where(x => x.ReadQuestionId.Equals(readQuestion.Id))
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();

                readQuestion.Selections = readChoiceSelections
                    .Where(x => x.ChoiceReadQuestionId.Equals(readQuestion.Id))
                    .OrderByDescending(x => x.Made)
                    .Take(1)
                    .ToObservable();
            }

            foreach (var readPart in readParts)
            {
                readPart.ReadQuestions = readQuestions
                    .Where(x => x.ReadPartId.Equals(readPart.Id))
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();

                readPart.ReadParagraphs = readParagraphs
                    .Where(x => x.ReadPartId.Equals(readPart.Id))
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();
            }

            foreach (var vocabQuestion in vocabQuestions)
            {
                vocabQuestion.VocabChoices = vocabChoices
                    .Where(x => x.VocabQuestionId.Equals(vocabQuestion.Id))
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();

                vocabQuestion.Selections = vocabChoiceSelections
                    .Where(x => x.ChoiceVocabQuestionId.Equals(vocabQuestion.Id))
                    .OrderByDescending(x => x.Made)
                    .Take(2)
                    .ToObservable();
            }

            foreach (var vocabPart in vocabParts)
                vocabPart.VocabQuestions = vocabQuestions
                    .Where(x => x.VocabPartId.Equals(vocabPart.Id))
                    .OrderBy(x => x.Ordinal)
                    .ToObservable();

            return new Assessment
            {
                Id = assignment.AssessmentId,
                Name = assignment.AssessmentName,
                AvailableStart = assignment.AssessmentAvailableStart,
                AvailableEnd = assignment.AssessmentAvailableEnd,
                Assignment = assignment,
                VocabParts = vocabParts.OrderBy(x => x.Ordinal).ToObservable(),
                ListenParts = listenParts.OrderBy(x => x.Ordinal).ToObservable(),
                ReadParts = readParts.OrderBy(x => x.Ordinal).ToObservable(),
            };
        });
    }

    private static AssessmentAssignment ReadAssessmentAssignment(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            StudentId = reader.ReadGuid(2),
            Assigned = reader.ReadDateTimeOffset(3),
            StartAfter = reader.ReadDateTimeOffset(4),
            EndBefore = reader.ReadDateTimeOffset(5),
            Started = reader.ReadDateTimeOffset(6),
            Completed = reader.ReadDateTimeOffset(7),
            Terminated = reader.ReadDateTimeOffset(8),
            AssessmentName = reader.ReadString(9),
            AssessmentAvailableStart = reader.ReadDateOnly(10),
            AssessmentAvailableEnd = reader.ReadDateOnly(11),
            StudentFirstName = reader.ReadString(12),
            StudentLastName = reader.ReadString(13),
            ListenPartCount = reader.ReadInt32(14),
            ListenQuestionCount = reader.ReadInt32(15),
            ListenChoiceSelectionCount = reader.ReadInt32(16),
            ListenPartCompletedCount = reader.ReadInt32(17),
            ListenTextEntryCount = reader.ReadInt32(18),
            ReadPartCount = reader.ReadInt32(19),
            ReadQuestionCount = reader.ReadInt32(20),
            ReadChoiceSelectionCount = reader.ReadInt32(21),
            ReadPartCompletedCount = reader.ReadInt32(22),
            ReadTextEntryCount = reader.ReadInt32(23),
            VocabPartCount = reader.ReadInt32(24),
            VocabQuestionCount = reader.ReadInt32(25),
            VocabChoiceSelectionCount = reader.ReadInt32(26),
            VocabPartCompletedCount = reader.ReadInt32(27),
            ImageFile = new()
            {
                Id = reader.ReadGuid(28),
                BlobId = reader.ReadGuid(29),
                Name = reader.ReadString(30),
                Format = reader.ReadString(31),
                Width = reader.ReadInt32(32),
                Height = reader.ReadInt32(33),
                Caption = reader.ReadString(34),
            },
        };
    }

    private static ListenPartCompleted ReadListenPartCompleted(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ListenPartId = reader.ReadGuid(2),
            DeviceTimestamp = reader.ReadDateTimeOffset(3),
        };
    }

    private static ReadPartCompleted ReadReadPartCompleted(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ReadPartId = reader.ReadGuid(2),
            DeviceTimestamp = reader.ReadDateTimeOffset(3),
        };
    }

    private static VocabPartCompleted ReadVocabPartCompleted(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            VocabPartId = reader.ReadGuid(2),
            DeviceTimestamp = reader.ReadDateTimeOffset(3),
        };
    }

    private static ListenPart ReadListenPart(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            PassageAudioFileId = reader.ReadGuid(3),
            PassageInstructionsText = reader.ReadString(4),
            PassageInstructionsAudioFileId = reader.ReadGuid(5),
            PreviewInstructionsText = reader.ReadString(6),
            PreviewInstructionsAudioFileId = reader.ReadGuid(7),
            QuestionsInstructionsText = reader.ReadString(8),
            QuestionsInstructionsAudioFileId = reader.ReadGuid(9),
            Ordinal = reader.ReadInt32(10),
            PassageAudioFileFile = new()
            {
                Id = reader.ReadGuid(11),
                BlobId = reader.ReadGuid(12),
                Name = reader.ReadString(13),
                Format = reader.ReadString(14),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(15),
                OptimizedBlobId = reader.ReadGuid(16),
                OptimizedFormat = reader.ReadString(17),
            },
            PassageInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(18),
                BlobId = reader.ReadGuid(19),
                Name = reader.ReadString(20),
                Format = reader.ReadString(21),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(22),
                OptimizedBlobId = reader.ReadGuid(23),
                OptimizedFormat = reader.ReadString(24),
            },
            PreviewInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(25),
                BlobId = reader.ReadGuid(26),
                Name = reader.ReadString(27),
                Format = reader.ReadString(28),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(29),
                OptimizedBlobId = reader.ReadGuid(30),
                OptimizedFormat = reader.ReadString(31),
            },
            QuestionsInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(32),
                BlobId = reader.ReadGuid(33),
                Name = reader.ReadString(34),
                Format = reader.ReadString(35),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(36),
                OptimizedBlobId = reader.ReadGuid(37),
                OptimizedFormat = reader.ReadString(38),
            },
            ListenQuestionCount = reader.ReadInt32(39),
        };
    }

    private static ReadPart ReadReadPart(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            PassageInstructionsText = reader.ReadString(3),
            PassageInstructionsAudioFileId = reader.ReadGuid(4),
            PreviewInstructionsText = reader.ReadString(5),
            PreviewInstructionsAudioFileId = reader.ReadGuid(6),
            QuestionsInstructionsText = reader.ReadString(7),
            QuestionsInstructionsAudioFileId = reader.ReadGuid(8),
            Ordinal = reader.ReadInt32(9),
            PassageInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(10),
                BlobId = reader.ReadGuid(11),
                Name = reader.ReadString(12),
                Format = reader.ReadString(13),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(14),
                OptimizedBlobId = reader.ReadGuid(15),
                OptimizedFormat = reader.ReadString(16),
            },
            PreviewInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(17),
                BlobId = reader.ReadGuid(18),
                Name = reader.ReadString(19),
                Format = reader.ReadString(20),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(21),
                OptimizedBlobId = reader.ReadGuid(22),
                OptimizedFormat = reader.ReadString(23),
            },
            QuestionsInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(24),
                BlobId = reader.ReadGuid(25),
                Name = reader.ReadString(26),
                Format = reader.ReadString(27),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(28),
                OptimizedBlobId = reader.ReadGuid(29),
                OptimizedFormat = reader.ReadString(30),
            },
            ReadParagraphCount = reader.ReadInt32(31),
            ReadQuestionCount = reader.ReadInt32(32),
        };
    }

    private static VocabPart ReadVocabPart(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            Title = reader.ReadString(2),
            PreviewInstructionsText = reader.ReadString(3),
            QuestionsInstructionsText = reader.ReadString(4),
            Ordinal = reader.ReadInt32(5),
            PreviewInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(6),
                BlobId = reader.ReadGuid(7),
                Name = reader.ReadString(8),
                Format = reader.ReadString(9),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(10),
                OptimizedBlobId = reader.ReadGuid(11),
                OptimizedFormat = reader.ReadString(12),
            },
            QuestionsInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(13),
                BlobId = reader.ReadGuid(14),
                Name = reader.ReadString(15),
                Format = reader.ReadString(16),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(17),
                OptimizedBlobId = reader.ReadGuid(18),
                OptimizedFormat = reader.ReadString(19),
            },
            VocabQuestionCount = reader.ReadInt32(20),
        };
    }

    private static ListenQuestion ReadListenQuestion(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ListenPartId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            IsPreview = reader.ReadBoolean(3),
            PageBreakBefore = reader.ReadBoolean(4),
            HasCorrectChoice = reader.ReadBoolean(5),
            RequireTextInput = reader.ReadBoolean(6),
            Ordinal = reader.ReadInt32(7),
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(8),
                BlobId = reader.ReadGuid(9),
                Name = reader.ReadString(10),
                Format = reader.ReadString(11),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(12),
                OptimizedBlobId = reader.ReadGuid(13),
                OptimizedFormat = reader.ReadString(14),
            },
            ImageFileFile = new()
            {
                Id = reader.ReadGuid(15),
                BlobId = reader.ReadGuid(16),
                Name = reader.ReadString(17),
                Format = reader.ReadString(18),
                Width = reader.ReadInt32(19),
                Height = reader.ReadInt32(20),
                Caption = reader.ReadString(21),
            },
        };
    }

    private static ListenChoice ReadListenChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ListenQuestionId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            IsCorrect = reader.ReadBoolean(3),
            ImageFileFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                Width = reader.ReadInt32(8),
                Height = reader.ReadInt32(9),
                Caption = reader.ReadString(10),
            },
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(11),
                BlobId = reader.ReadGuid(12),
                Name = reader.ReadString(13),
                Format = reader.ReadString(14),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(15),
                OptimizedBlobId = reader.ReadGuid(16),
                OptimizedFormat = reader.ReadString(17),
            },
            Ordinal = reader.ReadInt32(18),
        };
    }

    private static ListenChoiceSelection ReadListenChoiceSelection(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ChoiceId = reader.ReadGuid(2),
            Made = reader.ReadDateTimeOffset(3),
            ChoiceListenQuestionId = reader.ReadGuid(4),
            ChoiceText = reader.ReadString(5),
        };
    }

    private static ReadParagraph ReadReadParagraph(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ReadPartId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            Ordinal = reader.ReadInt32(3),
        };
    }

    private static ReadQuestion ReadReadQuestion(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ReadPartId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            IsPreview = reader.ReadBoolean(3),
            PageBreakBefore = reader.ReadBoolean(4),
            HasCorrectChoice = reader.ReadBoolean(5),
            RequireTextInput = reader.ReadBoolean(6),
            Ordinal = reader.ReadInt32(7),
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(8),
                BlobId = reader.ReadGuid(9),
                Name = reader.ReadString(10),
                Format = reader.ReadString(11),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(12),
                OptimizedBlobId = reader.ReadGuid(13),
                OptimizedFormat = reader.ReadString(14),
            },
            ImageFileFile = new()
            {
                Id = reader.ReadGuid(15),
                BlobId = reader.ReadGuid(16),
                Name = reader.ReadString(17),
                Format = reader.ReadString(18),
                Width = reader.ReadInt32(19),
                Height = reader.ReadInt32(20),
                Caption = reader.ReadString(21),
            },
        };
    }

    private static ReadChoice ReadReadChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ReadQuestionId = reader.ReadGuid(1),
            Text = reader.ReadString(2),
            IsCorrect = reader.ReadBoolean(3),
            ImageFileFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                Width = reader.ReadInt32(8),
                Height = reader.ReadInt32(9),
                Caption = reader.ReadString(10),
            },
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(11),
                BlobId = reader.ReadGuid(12),
                Name = reader.ReadString(13),
                Format = reader.ReadString(14),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(15),
                OptimizedBlobId = reader.ReadGuid(16),
                OptimizedFormat = reader.ReadString(17),
            },
            Ordinal = reader.ReadInt32(18),
        };
    }

    private static ReadChoiceSelection ReadReadChoiceSelection(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ChoiceId = reader.ReadGuid(2),
            Made = reader.ReadDateTimeOffset(3),
            ChoiceReadQuestionId = reader.ReadGuid(4),
            ChoiceText = reader.ReadString(5),
        };
    }

    private static VocabQuestion ReadVocabQuestion(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            VocabPartId = reader.ReadGuid(1),
            Word = reader.ReadString(2),
            IsPreview = reader.ReadBoolean(3),
            PageBreakBefore = reader.ReadBoolean(4),
            Ordinal = reader.ReadInt32(5),
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(6),
                BlobId = reader.ReadGuid(7),
                Name = reader.ReadString(8),
                Format = reader.ReadString(9),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(10),
                OptimizedBlobId = reader.ReadGuid(11),
                OptimizedFormat = reader.ReadString(12),
            },
        };
    }

    private static VocabChoice ReadVocabChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            VocabQuestionId = reader.ReadGuid(1),
            Word = reader.ReadString(2),
            IsCorrect = reader.ReadBoolean(3),
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(8),
                OptimizedBlobId = reader.ReadGuid(9),
                OptimizedFormat = reader.ReadString(10),
            },
            Ordinal = reader.ReadInt32(11),
        };
    }

    private static VocabChoiceSelection ReadVocabChoiceSelection(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssignmentId = reader.ReadGuid(1),
            ChoiceId = reader.ReadGuid(2),
            Made = reader.ReadDateTimeOffset(3),
            ChoiceVocabQuestionId = reader.ReadGuid(4),
            ChoiceText = reader.ReadString(5),
        };
    }
}
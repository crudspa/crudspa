namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabQuestionSelectForVocabPart
{
    public static async Task<IList<VocabQuestion>> Execute(String connection, Guid? sessionId, Guid? vocabPartId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabQuestionSelectForVocabPart";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@VocabPartId", vocabPartId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var vocabQuestions = new List<VocabQuestion>();

            while (await reader.ReadAsync())
                vocabQuestions.Add(ReadVocabQuestion(reader));

            await reader.NextResultAsync();

            var vocabChoices = new List<VocabChoice>();

            while (await reader.ReadAsync())
                vocabChoices.Add(ReadVocabChoice(reader));

            foreach (var vocabQuestion in vocabQuestions)
                vocabQuestion.VocabChoices = vocabChoices.Where(x => x.VocabQuestionId.Equals(vocabQuestion.Id)).ToObservable();

            return vocabQuestions;
        });
    }

    private static VocabQuestion ReadVocabQuestion(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            VocabPartId = reader.ReadGuid(1),
            VocabPartTitle = reader.ReadString(2),
            Word = reader.ReadString(3),
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
            IsPreview = reader.ReadBoolean(11),
            PageBreakBefore = reader.ReadBoolean(12),
            Ordinal = reader.ReadInt32(13),
        };
    }

    private static VocabChoice ReadVocabChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            VocabQuestionId = reader.ReadGuid(1),
            VocabQuestionWord = reader.ReadString(2),
            Word = reader.ReadString(3),
            IsCorrect = reader.ReadBoolean(4),
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(5),
                BlobId = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(9),
                OptimizedBlobId = reader.ReadGuid(10),
                OptimizedFormat = reader.ReadString(11),
            },
            Ordinal = reader.ReadInt32(12),
        };
    }
}
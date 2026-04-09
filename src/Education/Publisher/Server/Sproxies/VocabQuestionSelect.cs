namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class VocabQuestionSelect
{
    public static async Task<VocabQuestion?> Execute(String connection, Guid? sessionId, VocabQuestion vocabQuestion)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.VocabQuestionSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", vocabQuestion.Id);

        return await command.ExecuteQuery(connection, async reader =>
        {
            if (!await reader.ReadAsync())
                return null;

            vocabQuestion = ReadVocabQuestion(reader);

            await reader.NextResultAsync();

            while (await reader.ReadAsync())
                vocabQuestion.VocabChoices.Add(ReadVocabChoice(reader));

            return vocabQuestion;
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
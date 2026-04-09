namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadQuestionSelectForReadPart
{
    public static async Task<IList<ReadQuestion>> Execute(String connection, Guid? sessionId, Guid? readPartId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadQuestionSelectForReadPart";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ReadPartId", readPartId);

        return await command.ExecuteQuery(connection, async reader =>
        {
            var readQuestions = new List<ReadQuestion>();

            while (await reader.ReadAsync())
                readQuestions.Add(ReadReadQuestion(reader));

            await reader.NextResultAsync();

            var readChoices = new List<ReadChoice>();

            while (await reader.ReadAsync())
                readChoices.Add(ReadReadChoice(reader));

            foreach (var readQuestion in readQuestions)
                readQuestion.ReadChoices = readChoices.Where(x => x.ReadQuestionId.Equals(readQuestion.Id)).ToObservable();

            return readQuestions;
        });
    }

    private static ReadQuestion ReadReadQuestion(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ReadPartId = reader.ReadGuid(1),
            ReadPartTitle = reader.ReadString(2),
            Text = reader.ReadString(3),
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
            HasCorrectChoice = reader.ReadBoolean(13),
            RequireTextInput = reader.ReadBoolean(14),
            CategoryId = reader.ReadGuid(15),
            CategoryName = reader.ReadString(16),
            TypeId = reader.ReadGuid(17),
            TypeName = reader.ReadString(18),
            ImageFileFile = new()
            {
                Id = reader.ReadGuid(19),
                BlobId = reader.ReadGuid(20),
                Name = reader.ReadString(21),
                Format = reader.ReadString(22),
                Width = reader.ReadInt32(23),
                Height = reader.ReadInt32(24),
                Caption = reader.ReadString(25),
            },
            Ordinal = reader.ReadInt32(26),
        };
    }

    private static ReadChoice ReadReadChoice(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ReadQuestionId = reader.ReadGuid(1),
            ReadQuestionText = reader.ReadString(2),
            Text = reader.ReadString(3),
            IsCorrect = reader.ReadBoolean(4),
            ImageFileFile = new()
            {
                Id = reader.ReadGuid(5),
                BlobId = reader.ReadGuid(6),
                Name = reader.ReadString(7),
                Format = reader.ReadString(8),
                Width = reader.ReadInt32(9),
                Height = reader.ReadInt32(10),
                Caption = reader.ReadString(11),
            },
            AudioFileFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(16),
                OptimizedBlobId = reader.ReadGuid(17),
                OptimizedFormat = reader.ReadString(18),
            },
            Ordinal = reader.ReadInt32(19),
        };
    }
}
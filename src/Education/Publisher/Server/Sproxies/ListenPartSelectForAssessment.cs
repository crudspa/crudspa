namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ListenPartSelectForAssessment
{
    public static async Task<IList<ListenPart>> Execute(String connection, Guid? sessionId, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ListenPartSelectForAssessment";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadListenPart);
    }

    private static ListenPart ReadListenPart(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            AssessmentId = reader.ReadGuid(1),
            AssessmentName = reader.ReadString(2),
            Title = reader.ReadString(3),
            PassageAudioFileFile = new()
            {
                Id = reader.ReadGuid(4),
                BlobId = reader.ReadGuid(5),
                Name = reader.ReadString(6),
                Format = reader.ReadString(7),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(8),
                OptimizedBlobId = reader.ReadGuid(9),
                OptimizedFormat = reader.ReadString(10),
            },
            PassageInstructionsText = reader.ReadString(11),
            PassageInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(12),
                BlobId = reader.ReadGuid(13),
                Name = reader.ReadString(14),
                Format = reader.ReadString(15),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(16),
                OptimizedBlobId = reader.ReadGuid(17),
                OptimizedFormat = reader.ReadString(18),
            },
            PreviewInstructionsText = reader.ReadString(19),
            PreviewInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(20),
                BlobId = reader.ReadGuid(21),
                Name = reader.ReadString(22),
                Format = reader.ReadString(23),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(24),
                OptimizedBlobId = reader.ReadGuid(25),
                OptimizedFormat = reader.ReadString(26),
            },
            QuestionsInstructionsText = reader.ReadString(27),
            QuestionsInstructionsAudioFileFile = new()
            {
                Id = reader.ReadGuid(28),
                BlobId = reader.ReadGuid(29),
                Name = reader.ReadString(30),
                Format = reader.ReadString(31),
                OptimizedStatus = reader.ReadEnum<AudioFile.OptimizationStatus>(32),
                OptimizedBlobId = reader.ReadGuid(33),
                OptimizedFormat = reader.ReadString(34),
            },
            Ordinal = reader.ReadInt32(35),
            ListenQuestionCount = reader.ReadInt32(36),
        };
    }
}
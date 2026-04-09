namespace Crudspa.Education.School.Server.Sproxies;

public static class ListenPartSelectForAssessment
{
    public static async Task<IList<ListenPart>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ListenPartSelectForAssessment";

        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadListenPart);
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
}
namespace Crudspa.Education.School.Server.Sproxies;

public static class ReadParagraphSelectForAssessment
{
    public static async Task<IList<ReadParagraph>> Execute(String connection, Guid? assessmentId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationSchool.ReadParagraphSelectForAssessment";

        command.AddParameter("@AssessmentId", assessmentId);

        return await command.ReadAll(connection, ReadReadParagraph);
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
}
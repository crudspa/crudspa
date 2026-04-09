namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadParagraphSelect
{
    public static async Task<ReadParagraph?> Execute(String connection, Guid? sessionId, ReadParagraph readParagraph)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadParagraphSelect";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readParagraph.Id);

        return await command.ReadSingle(connection, ReadReadParagraph);
    }

    private static ReadParagraph ReadReadParagraph(SqlDataReader reader)
    {
        return new()
        {
            Id = reader.ReadGuid(0),
            ReadPartId = reader.ReadGuid(1),
            ReadPartTitle = reader.ReadString(2),
            Text = reader.ReadString(3),
            Ordinal = reader.ReadInt32(4),
        };
    }
}
namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadParagraphSelectForReadPart
{
    public static async Task<IList<ReadParagraph>> Execute(String connection, Guid? sessionId, Guid? readPartId)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadParagraphSelectForReadPart";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ReadPartId", readPartId);

        return await command.ReadAll(connection, ReadReadParagraph);
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
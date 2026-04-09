namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadParagraphDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadParagraph readParagraph)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadParagraphDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readParagraph.Id);

        await command.Execute(connection, transaction);
    }
}
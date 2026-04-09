namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadParagraphUpdate
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadParagraph readParagraph)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadParagraphUpdate";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", readParagraph.Id);
        command.AddParameter("@Text", readParagraph.Text);

        await command.Execute(connection, transaction);
    }
}
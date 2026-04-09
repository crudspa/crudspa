namespace Crudspa.Education.Publisher.Server.Sproxies;

public static class ReadParagraphInsert
{
    public static async Task<Guid?> Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, ReadParagraph readParagraph)
    {
        await using var command = new SqlCommand();
        command.CommandText = "EducationPublisher.ReadParagraphInsert";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@ReadPartId", readParagraph.ReadPartId);
        command.AddParameter("@Text", readParagraph.Text);

        var output = command.AddOutputParameter("@Id");
        await command.Execute(connection, transaction);
        return (Guid?)output.Value;
    }
}
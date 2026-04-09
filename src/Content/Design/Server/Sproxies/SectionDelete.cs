namespace Crudspa.Content.Design.Server.Sproxies;

public static class SectionDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, Section section)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.SectionDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", section.Id);

        await command.Execute(connection, transaction);
    }
}
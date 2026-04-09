namespace Crudspa.Content.Design.Server.Sproxies;

public static class MultimediaElementDelete
{
    public static async Task Execute(SqlConnection connection, SqlTransaction? transaction, Guid? sessionId, MultimediaElement multimediaElement)
    {
        await using var command = new SqlCommand();
        command.CommandText = "ContentDesign.MultimediaElementDelete";

        command.AddParameter("@SessionId", sessionId);
        command.AddParameter("@Id", multimediaElement.Id);

        await command.Execute(connection, transaction);
    }
}